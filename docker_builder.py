import subprocess
import sys

help_msg = '''
Usage: python docker_builder.py [ACTION] [SERVICE_ARGUMENT]

Actions:
  -h, --help            Shows this help message
  -b, --build           Builds specified image(s) (Default action if not specified)
  -d, --delete          Deletes specified image(s) and cleans up containers
  -r, --run             Runs whole composition of containers at the end
  -rd, --run_detached   Runs whole composition of containers at the end (detached)

Service arguments (Optional):
  -api                  Target: webapi image

Examples:
  python docker_builder.py --build -api       -> Builds only the webapi image
  python docker_builder.py -b -api            -> Builds only the webapi image
  python docker_builder.py -d -api            -> Deletes the webapi image
  python docker_builder.py                    -> Builds all images (Default behavior)
  python docker_builder.py -r                 -> Builds all images and runs whole composition at the end
  python docker_builder.py -rd                -> Builds all images and runs whole composition at the end (detached)
  python docker_builder.py --delete           -> Deletes all listed images
'''

colors = {
    "red": "\033[91m",
    "green": "\033[92m",
    "yellow": "\033[93m",
    "blue": "\033[94m",
    "white": "\033[0m"
}

script_tags = {
    "-b": "--build",
    "-d": "--delete",
    "-r": "--run",
    "-rd": "--run_detached"
}

services = {
    # "-api": ("./src/backend/WebAPI", "web_api"),
    "-db_init": ("./src/db/DatabaseInitializer", "db_initializer")
}

services_to_build = 0
built_services = 0

def run_command(command):
    try:
        subprocess.run(command, text=True, check=True)
        return True
    except subprocess.CalledProcessError:
        return False
    except FileNotFoundError:
        print(f"{colors['red']}\n[ERROR]{colors['white']} Docker not found")
        sys.exit(1)


def build_docker_image(service_key):
    dockerfile_path, tag = services[service_key]
    command = ["docker", "build", "-f", dockerfile_path + "/Dockerfile", "-t", tag, dockerfile_path]

    print(f"{colors['blue']}Running:{colors['white']} {' '.join(command)}")

    try:
        subprocess.run(command, text=True, check=True)
        print(
            f"{colors['green']}[SUCCESS {built_services}/{services_to_build}]{colors['white']} Image '{tag}' built successfully\n")
    except subprocess.CalledProcessError as e:
        print(f"{colors['red']}\n[ERROR]{colors['white']} Docker build finished with error code {e.returncode}")
        sys.exit(e.returncode)


def delete_docker_image(service_key):
    _, tag = services[service_key]
    print(f"{colors['yellow']}Cleaning up service:{colors['white']} {tag}")

    run_command(["docker", "rm", "-f", tag])

    remove_cmd = ["docker", "rmi", tag]
    print(f"{colors['blue']}Running:{colors['white']} {' '.join(remove_cmd)}")

    if run_command(remove_cmd):
        print(
            f"{colors['green']}[SUCCESS {built_services}/{services_to_build}]{colors['white']} Image '{tag}' removed successfully\n")
    else:
        print(
            f"{colors['red']}[WARNING {built_services}/{services_to_build}]{colors['white']} Could not remove image '{tag}'\n")


if __name__ == "__main__":
    args = sys.argv[1:]

    if "-h" in args or "--help" in args:
        print(help_msg)
        sys.exit(0)

    action = script_tags['-b']
    selected_services = []
    run_flag = False
    run_detached = False

    for arg in args:
        if arg in script_tags.values():
            if arg == "--run" or arg =="--run_detached":
                run_flag = True
                if arg == "--run_detached":
                    run_detached = True
                continue
            action = arg
        elif arg in script_tags.keys():
            arg = script_tags[arg]
            if arg == "--run" or arg =="--run_detached":
                run_flag = True
                if arg == "--run_detached":
                    run_detached = True
                continue
            action = arg
        elif arg in services:
            selected_services.append(arg)
        else:
            print(f"{colors['red']}[ERROR] Invalid argument: {arg}{colors['white']}")
            print(help_msg)
            sys.exit(1)

    if not selected_services:
        selected_services = list(services.keys())

    services_to_build = len(selected_services)
    for service in selected_services:
        if action == "--build":
            built_services += 1
            build_docker_image(service)
        elif action == "--delete":
            built_services += 1
            delete_docker_image(service)

    print(colors["white"])
    if run_flag:
        if run_detached:
            compose_up_command = ["docker", "compose", "up", "-d"]
        else:
            compose_up_command = ["docker", "compose", "up"]
        print(f"{colors['green']}[SUCCESS]{colors['white']} All images built successfully, running Docker compose\n")
        print(f"{colors['blue']}Running:{colors['white']} {' '.join(compose_up_command)}")
        run_command(compose_up_command)
