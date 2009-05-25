/* texture.h */
/* vim: set shiftwidth=4 cindent : */

typedef struct {
    char name[256];
    char filename[256];
    int width;
    int height;
    int depth;
    unsigned char *data;
} Texture;

Texture *create_texture();
void texture_read(Texture *texture, FILE *file);
void texture_dump(Texture *texture);
void free_texture(Texture *texture);
