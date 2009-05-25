/* effect.h */
/* vim: set shiftwidth=4 cindent : */

typedef struct {
    char name[256];
    int nlines;
    char **lines;
} Effect;

Effect *create_effect();
void effect_read(Effect *effect, FILE *file);
void effect_dump(Effect *effect);
void free_effect(Effect *effect);
