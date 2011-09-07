#!/usr/bin/env python
import subprocess

NvTriStripPath = "NvTriStripper-cli.exe"

def optimize(vert_indices):
	# Stripifying mesh
	NvTriStrip = subprocess.Popen([NvTriStripPath], 
		stdin = subprocess.PIPE, stdout = subprocess.PIPE)
	for vi in vert_indices:
		NvTriStrip.stdin.write(str(vi) + " ")
	NvTriStrip.stdin.write("-1\n")
	stripcount = int(NvTriStrip.stdout.readline())
	if stripcount < 1:
		raise Exception("NvTriStrip returned 0 strips. Aborting")
	nativelist = []
	striptype = int(NvTriStrip.stdout.readline())
	nativelength = int(NvTriStrip.stdout.readline())
	nativelist.extend(map(int, NvTriStrip.stdout.readline().split()))
	return nativelist

if __name__ == "__main__":
	print optimize([ 0, 1, 2, 1, 3, 2 ])

# vim: set sw=4 ts=4:
