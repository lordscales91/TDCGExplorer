/* mesh.h */
/* vim: set shiftwidth=4 cindent : */

typedef struct {
    int spec;
    int nbones;
    int *bones;
    int nverts;
    Vertex *vertices;
    int nindices;
    unsigned short *indices;
} Submesh;

Submesh *create_sub();
void sub_read(Submesh *sub, FILE *file);
void sub_dump(Submesh *sub);
void free_sub(Submesh *sub);

typedef struct {
    char name[256];
    Matrix matrix;
    int effect;
    int nsubs;
    Submesh **subs;
    unsigned int *bufObjs;
    unsigned int *bufObje;
} Mesh;

Mesh *create_mesh();
void mesh_read(Mesh *mesh, FILE *file);
void mesh_dump(Mesh *mesh);
void free_mesh(Mesh *mesh);
