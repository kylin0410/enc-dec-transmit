#!/usr/bin/python
import json
import os
import re
import sys
import traceback
import requests
import base64
import gzip
import socket
import config
import psutil

from Crypto.PublicKey import RSA
from Crypto.Cipher import PKCS1_v1_5


def post_bytes(api_url, json_str, timeout=3):
    """
    @summary: Post bytearray data.
    @param api_url: [string] Web API URL.
    @param json_str: [str] Json string.
    @param timeout: [int] Timeout, maximum execution time and logging timeout.
    @return: [int] result. 0: successfully, -1: exec failure.
    """
    res = -1
    resp = None
    header = dict()
    # Don't encrypt by default.
    header["Content-Type"] = "application/octet-stream"
    if not config.ENABLE_ENCRYPT:
        header["Content-Type"] = "application/gzip"
    try:
        # Compress data.
        b64_array = base64.b64encode(bytes(u'{}'.format(json_str), "utf-8"))  # String to base64 byte array.
        com_array = gzip.compress(b64_array)  # Base64 byte array to compressed byte array.
        
        # Encrypt handle or not.
        if not config.ENABLE_ENCRYPT:
            resp = requests.post(api_url, data=com_array, headers=header, verify=False, timeout=timeout)
        else:
            enc_array = b""
            rsa = RSA.importKey(config.PUB_KEY.encode())
            cipher = PKCS1_v1_5.new(rsa)
            for i in range(0, len(com_array), 100):
                enc_block = cipher.encrypt(com_array[i:i+100]);
                enc_array = enc_array + enc_block
            resp = requests.post(api_url, data=enc_array, headers=header, verify=False, timeout=timeout)

        # Result.
        if resp.status_code == requests.codes.ok:
            res = 0
        len_ori = len(json_str)
        len_com = len(com_array)
        ratio = (len_ori - len_com)/len_ori*100
    except:
        tp, value, tb = sys.exc_info()
        #raise # Execute command always return res, out, err. Upper layer use res == 0 to know the result.
    finally:
        if res != 0:
            if resp:
                if "System.Convert.FromBase64String" in resp.text:
                    config.ENABLE_ENCRYPT = False
    return res


if __name__ == "__main__":
    json = \
    {
        "DeviceName": "test",
        "UUID": "5e43e72f-cd40-46e4-a245-bc57993d1ea8",
        "UTCDT": datetime.datetime.utcnow().strftime("%Y-%m-%d %H:%M:%S")
    }
    jsonStr = json.dumps(hb_data)
    post_bytes("http://localhost:5001/api/Heartbeat", jsonStr)
    pass
