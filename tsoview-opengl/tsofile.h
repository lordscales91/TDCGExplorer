/* tsofile.h */
/* vim: set shiftwidth=4 cindent : */

#include "texture.h"
#include "effect.h"
#include "param.h"
#include "material.h"
#include "mesh.h"

typedef struct st_node {
    struct st_node *parent;
    struct st_node *children_head;
    struct st_node *children_next;
    char *name;
    char *short_name;
    Matrix transform;
    Matrix offset;
    Matrix combined;
} Node;

typedef struct {
    int nnodes;
    Node **nodes;
    int nmatrices;
    Matrix *matrices;
    int ntextures;
    Texture **textures;
    int neffects;
    Effect **effects;
    int nmaterials;
    Material **materials;
    int nmeshes;
    Mesh **meshes;
} Tsofile;

Tsofile *create_tso();
void tso_read(Tsofile *tso, FILE *file);
void tso_dump(Tsofile *tso);
void free_tso(Tsofile *tso);
