/* tsofile.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "matrix.h"
#include "tdcg.h"
#include "tsofile.h"

Tsofile *create_tso()
{
    Tsofile *tso = (Tsofile *)malloc(sizeof(Tsofile));
    return tso;
}

void node_tree(Node *node, int lev)
{
    int i;
    for (i=0; i<lev; i++)
	printf("  ");
    printf("%s\n", node->short_name);

    Node *bone = node->children_head;
    while (bone != NULL)
    {
	node_tree(bone, lev+1);
	bone = bone->children_next;
    }
}

void node_make_combined_matrix(Node *node, Matrix m)
{
    multMatrix(node->combined.m, m.m, node->transform.m);

    Node *bone = node->children_head;
    while (bone != NULL)
    {
	node_make_combined_matrix(bone, node->combined);
	bone = bone->children_next;
    }
}

void tso_read(Tsofile *tso, FILE *file)
{
    tso->nnodes = read_int(file);
    tso->nodes = (Node **)malloc(sizeof(Node *)*tso->nnodes);
    int i;
    for (i=0; i<tso->nnodes; i++)
    {
	Node *node = tso->nodes[i] = (Node *)malloc(sizeof(Node));
	node->parent = NULL;
	node->children_head = NULL;
	node->children_next = NULL;
	node->name = (char *)malloc(sizeof(char)*256);
	node->short_name = NULL;
	read_string(node->name, file);
    }
    for (i=0; i<tso->nnodes; i++)
    {
	Node *node = tso->nodes[i];

	/* make short name */
    {
	char *p = strrchr(node->name, '|');
	char *e = node->name + strlen(node->name);
	int len = e - p;
	node->short_name = (char *)malloc(sizeof(char)*(len+1));
	memcpy(node->short_name, p+1, len);
	node->short_name[len] = '\0';
	//printf("short name %s\n", node->short_name);
    }

	/* make parent name */
	char *p = strrchr(node->name, '|');
	int len = p - node->name;

	char parent_name[256];
	memcpy(parent_name, node->name, len);
	parent_name[len] = '\0';

	/* find and assign parent and children */
	int j;
	for (j=0; j<tso->nnodes; j++)
	{
	    Node *parent = tso->nodes[j];

	    if (!strcmp(parent_name, parent->name))
	    {
		node->parent = parent;
		node->children_next = node->parent->children_head;
		node->parent->children_head = node;
		break;
	    }
	}
    }
    /*
    {
	Node *root = tso->nodes[0];
	node_tree(root, 0);
    }
    */

    tso->nmatrices = read_int(file);
    tso->matrices = (Matrix *)malloc(sizeof(Matrix)*tso->nmatrices);
    Matrix *m = tso->matrices;
    for (i=0; i<tso->nmatrices; i++, m++)
    {
	read_matrix(m, file);
    }

    for (i=0; i<tso->nnodes; i++)
    {
	Node *node = tso->nodes[i];

	/* assign node transform matrix */
	node->transform = tso->matrices[i];
    }

    for (i=0; i<tso->nnodes; i++)
    {
	Node *node = tso->nodes[i];

	/* make offset matrix */
	Matrix m;
	makeIdentityMatrix(m.m);

	Node *bone = node;
	while (bone != NULL)
	{
	    multMatrix(m.m, bone->transform.m, m.m);
	    bone = bone->parent;
	}
	invertMatrix(node->offset.m, m.m);
    }
    {
	Matrix m;
	makeIdentityMatrix(m.m);

	Node *root = tso->nodes[0];
	node_make_combined_matrix(root, m);
    }

    tso->ntextures = read_int(file);
    tso->textures = (Texture **)malloc(sizeof(Texture *)*tso->ntextures);
    for (i=0; i<tso->ntextures; i++)
    {
	Texture *texture = tso->textures[i] = create_texture();
	texture_read(texture, file);
    }

    tso->neffects = read_int(file);
    tso->effects = (Effect **)malloc(sizeof(Effect *)*tso->neffects);
    for (i=0; i<tso->neffects; i++)
    {
	Effect *effect = tso->effects[i] = create_effect();
	effect_read(effect, file);
    }

    tso->nmaterials = read_int(file);
    tso->materials = (Material **)malloc(sizeof(Material *)*tso->nmaterials);
    for (i=0; i<tso->nmaterials; i++)
    {
	Material *material = tso->materials[i] = create_material();
	material_read(material, file);
    }

    tso->nmeshes = read_int(file);
    tso->meshes = (Mesh **)malloc(sizeof(Mesh *)*tso->nmeshes);
    for (i=0; i<tso->nmeshes; i++)
    {
	Mesh *mesh = tso->meshes[i] = create_mesh();
	mesh_read(mesh, file);
    }
}

void tso_dump(Tsofile *tso)
{
    printf("nnodes %d\n", tso->nnodes);
    printf("%s\n", tso->nodes[tso->nnodes-1]->name);
    printf("nmatrices %d\n", tso->nmatrices);
    printf("ntextures %d\n", tso->ntextures);
    int i;
    for (i=0; i<tso->ntextures; i++)
    {
	Texture *texture = tso->textures[i];
	texture_dump(texture);
    }
    printf("neffects %d\n", tso->neffects);
    for (i=0; i<tso->neffects; i++)
    {
	Effect *effect = tso->effects[i];
	effect_dump(effect);
    }
    printf("nmaterials %d\n", tso->nmaterials);
    for (i=0; i<tso->nmaterials; i++)
    {
	Material *material = tso->materials[i];
	material_dump(material);
    }
    printf("nmeshes %d\n", tso->nmeshes);
    for (i=0; i<tso->nmeshes; i++)
    {
	Mesh *mesh = tso->meshes[i];
	mesh_dump(mesh);
    }
}

void free_tso(Tsofile *tso)
{
    free(tso->matrices);
    int i;
    for (i=0; i<tso->nmeshes; i++)
    {
	Mesh *mesh = tso->meshes[i];
	free_mesh(mesh);
    }
    free(tso->meshes);
    for (i=0; i<tso->nmaterials; i++)
    {
	Material *material = tso->materials[i];
	free_material(material);
    }
    free(tso->materials);
    for (i=0; i<tso->neffects; i++)
    {
	Effect *effect = tso->effects[i];
	free_effect(effect);
    }
    free(tso->effects);
    for (i=0; i<tso->ntextures; i++)
    {
	Texture *texture = tso->textures[i];
	free_texture(texture);
    }
    free(tso->textures);
    for (i=0; i<tso->nnodes; i++)
    {
	Node *node = tso->nodes[i];
	free(node->short_name);
	free(node->name);
	free(node);
    }
    free(tso->nodes);
    free(tso);
}

