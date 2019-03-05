import os
import boto3

def walkdir(dirname):
    for cur, _dirs, files in os.walk(dirname):
        pref = ''
        head, tail = os.path.split(cur)
        while head:
            pref += '---'
            head, _tail = os.path.split(head)
        #print(pref+tail)
        for f in files:
            #print(pref+'---'+f)
            s3 = boto3.resource('s3')
            BUCKET = "sooyunkim-bucket"
            s3.Bucket(BUCKET).upload_file(walkdir('.'))



