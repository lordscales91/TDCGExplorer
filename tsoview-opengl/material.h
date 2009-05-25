/* material.h */
/* vim: set shiftwidth=4 cindent : */

typedef struct {
    char name[256];
    char filename[256];
    int nlines;
    char **lines;
} Material;

Material *create_material();
void material_read(Material *material, FILE *file);
void material_dump(Material *material);
void free_material(Material *material);
