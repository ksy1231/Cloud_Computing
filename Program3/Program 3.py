import os
import boto3

s3 = boto3.client('s3')

def uploadDirectory(path, bucketname):
    for root, dirs, files in os.walk(path):
        for file in files:
            s3.upload_file(os.path.join(root, file), bucketname, file)
            
def walkdir(dirname):
    for cur, _dirs, files in os.walk(dirname):
        head, tail = os.path.split(cur)
        while head:
            head, _tail = os.path.split(head)
        uploadDirectory(tail, "sooyunkim-bucket")
        
walkdir('.')
