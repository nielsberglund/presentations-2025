# Unlocking the Magic: An Intro to GenAI & LLMs

## Set up the environment

Create a Conda environment:

```bash
$ conda create --name pytorch python=3.12
```

Activate the environment:

```bash
$ conda activate pytorch
```

Test that it worked:

```bash
$ python --version
```

If you want to run from Jupyter notebook install `ipykernel`:

```bash
$  conda install -n pytorch ipykernel --update-deps --force-reinstall
```

You also need to install `ipywidgets`

```bash
$ pip install ipywidgets
```

Install the Hugging Face Transformers library, and PyTorch:

```bash
$ pip install transformers
$ pip install torch
```

Check that it works; from command line:

```bash
$ python
>>> import torch
>>> exit()
```

If you are on Windows 11 and you get an error looking something like so:

```bash
OSError: [WinError 126] The specified module could not be found. 
Error loading "C:...\site-packages\torch\lib\fbgemm.dll"”" 
or one of its dependencies.
```

That error is most likely due to you do not have `libomp140.x86_64.dll` on your machine. You can download the dll from: `https://www.dllme.com/dll/files/libomp140_x86_64?sort=upload&arch=0x8664`. After having downloaded it you put it in your `Windows\system32` directory and all should now be fine.

If you want to use OpenAI you:

```bash
$ pip install openai
```

When the above is done you can now run code.


## Code

