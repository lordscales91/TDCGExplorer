/* param.h */
/* vim: set shiftwidth=4 cindent : */

typedef struct {
    char *type;
    char *name;
    char *value;
} Param;

Param *create_param();
void free_param(Param *param);
void param_read(Param *param, char *line);
void param_dump(Param *param);
