/* mesh.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>

#include "tdcg.h"
#include "mesh.h"

void mesh_read(Mesh *mesh, FILE *file)
{
    read_string(mesh->name, file);
    read_matrix(&mesh->matrix, file);
    mesh->effect = read_int(file);
    mesh->nsubs = read_int(file);
    mesh->subs = (Submesh **)malloc(sizeof(Submesh *)*mesh->nsubs);
    int j;
    for (j=0; j<mesh->nsubs; j++)
    {
	Submesh *sub = mesh->subs[j] = create_sub();
	sub_read(sub, file);
    }
}

void mesh_dump(Mesh *mesh)
{
    printf("name %s\n", mesh->name);
    printf("effect %d\n", mesh->effect);
    printf("nsubs %d\n", mesh->nsubs);
    int j;
    for (j=0; j<mesh->nsubs; j++)
    {
	Submesh *sub = mesh->subs[j];
	sub_dump(sub);
    }
}

Mesh *create_mesh()
{
    Mesh *mesh = (Mesh *)malloc(sizeof(Mesh));
    mesh->bufObjs = NULL;
    mesh->bufObje = NULL;
    return mesh;
}

void free_mesh(Mesh *mesh)
{
    if (mesh->bufObje)
	free(mesh->bufObje);
    if (mesh->bufObjs)
	free(mesh->bufObjs);
    int j;
    for (j=0; j<mesh->nsubs; j++)
    {
	free_sub(mesh->subs[j]);
    }
    free(mesh->subs);
    free(mesh);
}

Submesh *create_sub()
{
    Submesh *sub = (Submesh *)malloc(sizeof(Submesh));
    sub->nindices = 0;
    sub->indices = NULL;
    return sub;
}

void sub_read(Submesh *sub, FILE *file)
{
    sub->spec = read_int(file);
    sub->nbones = read_int(file);
    sub->bones = (int *)malloc(sizeof(int)*sub->nbones);
    int *b = sub->bones;
    int k;
    for (k=0; k<sub->nbones; k++, b++)
    {
	*b = read_int(file);
    }
    sub->nverts = read_int(file);
    sub->vertices = (Vertex *)malloc(sizeof(Vertex)*sub->nverts);
    Vertex *v = sub->vertices;
    for (k=0; k<sub->nverts; k++, v++)
    {
	read_vertex(v, file);
    }
}

void sub_dump(Submesh *sub)
{
    printf("spec %d\n", sub->spec);
    printf("nbones %d\n", sub->nbones);
}

void free_sub(Submesh *sub)
{
    if (sub->indices)
	free(sub->indices);
    free(sub->bones);
    free(sub->vertices);
    free(sub);
}
