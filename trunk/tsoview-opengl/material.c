/* material.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>

#include "tdcg.h"
#include "material.h"

void material_read(Material *material, FILE *file)
{
    read_string(material->name, file);
    read_string(material->filename, file);
    material->nlines = read_int(file);
    material->lines = (char **)malloc(sizeof(Material *)*material->nlines);
    int l;
    for (l=0; l<material->nlines; l++)
    {
	char *line = material->lines[l] = (char *)malloc(sizeof(char)*256);
	read_string(line, file);
    }
}

void material_dump(Material *material)
{
    printf("name %s\n", material->name);
    printf("filename %s\n", material->filename);
    printf("nlines %d\n", material->nlines);
}

Material *create_material()
{
    Material *material = (Material *)malloc(sizeof(Material));
    return material;
}

void free_material(Material *material)
{
    int l;
    for (l=0; l<material->nlines; l++)
    {
	char *line = material->lines[l];
	free(line);
    }
    free(material->lines);
    free(material);
}
