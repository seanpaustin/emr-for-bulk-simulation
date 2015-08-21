#!/usr/bin/python

import sys

counts={}
for line in sys.stdin:
    key = line.rstrip('\n')
    counts[key] = counts.get(key, 0) + 1 


for k,v in counts.iteritems():
    print k, v
