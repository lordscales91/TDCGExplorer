/* texture.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>

#include "tdcg.h"
#include "texture.h"

void texture_read(Texture *texture, FILE *file)
{
    read_string(texture->name, file);
    read_string(texture->filename, file);
    texture->width = read_int(file);
    texture->height = read_int(file);
    texture->depth = read_int(file);
    int size = texture->width*texture->height*texture->depth;
    texture->data = (unsigned char*)malloc(size);
    fread(texture->data, sizeof(unsigned char)*size, 1, file);
}

void texture_dump(Texture *texture)
{
    printf("name %s\n", texture->name);
    printf("filename %s\n", texture->filename);
    printf("width %d height %d depth %d\n", texture->width, texture->height, texture->depth);
}

Texture *create_texture()
{
    Texture *texture = (Texture *)malloc(sizeof(Texture));
    return texture;
}

void free_texture(Texture *texture)
{
    free(texture->data);
    free(texture);
}
