import json
import os
from pathlib import Path
from typing import Type, List, TypeVar

JSON_EXTENSION = ".json"
CURRENT_DIR = Path(__file__).resolve().parent
RES_PATH = CURRENT_DIR.parent.parent / "res"
T = TypeVar('T')


def open_json_file(filename: str, model_class: Type[T]) -> List[T]:
    file_path = os.path.join(RES_PATH, filename + JSON_EXTENSION)
    with open(file_path, 'r', encoding='utf-8') as file:
        data_dictionary = json.load(file)

    if isinstance(data_dictionary, list):
        return [model_class(**item) for item in data_dictionary]
    else:
        return [model_class(**data_dictionary)]
