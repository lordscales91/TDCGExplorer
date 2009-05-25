/* effect.c */
/* vim: set shiftwidth=4 cindent : */
#include <stdio.h>
#include <stdlib.h>

#include "tdcg.h"
#include "effect.h"

void effect_read(Effect *effect, FILE *file)
{
    read_string(effect->name, file);
    effect->nlines = read_int(file);
    effect->lines = (char **)malloc(sizeof(char *)*effect->nlines);
    int l;
    for (l=0; l<effect->nlines; l++)
    {
	char *line = effect->lines[l] = (char *)malloc(sizeof(char)*256);
	read_string(line, file);
    }
}

void effect_dump(Effect *effect)
{
    printf("name %s\n", effect->name);
    printf("nlines %d\n", effect->nlines);
}

Effect *create_effect()
{
    Effect *effect = (Effect *)malloc(sizeof(Effect));
    return effect;
}

void free_effect(Effect *effect)
{
    int l;
    for (l=0; l<effect->nlines; l++)
    {
	char *line = effect->lines[l];
	free(line);
    }
    free(effect->lines);
    free(effect);
}
