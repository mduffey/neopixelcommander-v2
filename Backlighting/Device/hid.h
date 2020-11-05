


#ifdef __cplusplus
extern "C" {
#endif


#ifdef BUILDING_EXAMPLE_DLL
#define EXAMPLE_DLL __declspec(dllexport)
#else
#define EXAMPLE_DLL 
#endif


int __stdcall EXAMPLE_DLL rawhid_open(int max, int vid, int pid, int usage_page, int usage);
int __stdcall EXAMPLE_DLL rawhid_recv(int num, void *buf, int len, int timeout);
int __stdcall EXAMPLE_DLL rawhid_send(int num, void *buf, int len, int timeout);
void __stdcall EXAMPLE_DLL rawhid_close(int num);

#ifdef __cplusplus
}
#endif
