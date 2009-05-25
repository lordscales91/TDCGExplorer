/* tmodump.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>

#include "tdcg.h"
#include "tmofile.h"

int main(int argc, char *argv[])
{
    FILE *file;

    if (argc != 2)
    {
	printf("Usage: tmodump <tmo file>\n");
	return 1;
    }

    char *filename = argv[1];
    puts(filename);
    
    file = fopen(filename, "rb");
    if (!file)
	return 1;

    int magic;
    magic = read_int(file);
    printf("magic %08x\n", magic);
    if (magic != (int)(('T'<<8*0)+('M'<<8*1)+('O'<<8*2)+('1'<<8*3)))
    {
	fclose(file);
	return 1;
    }

    Tmofile *tmo = create_tmo();
    tmo_read(tmo, file);
    tmo_dump(tmo);
    free_tmo(tmo);
    tmo = NULL;

    fclose(file);
    return 0;
}
