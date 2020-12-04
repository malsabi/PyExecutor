# PyExecutor

### About
Python Executor, it's fully written in C# that can enable to run python scripts and obtain the results multiple times simentiniously using sessions.

### Properties
  1. Can run multiple instances (child) that are connected to main process (parent) which is the PyExecutor.
  2. Can run different type of python scripts, it's not specified.
  3. Sessions can be authenticated and may connect on different host machines.
  4. Buffering is enabled by default on sessions to increase the speed of transition.
 
### Requirements

  1. GPU Nvidia supporting cuda to increase the performance of the python scripts if its using Machine learning modules.
  2. CPU Inteli7 Ghz2.5+
  3. RAM 16GB, 32GB if using Cuda
  4. Windows 7 and above since the PyExecutor is using Framework 4.5+.
