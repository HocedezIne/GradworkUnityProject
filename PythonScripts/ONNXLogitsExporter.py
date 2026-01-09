import sys
import onnx
from onnx import helper, TensorProto
from pathlib import Path

try:
    model_path = Path(input("Provide path to onnx model: ").strip())
    if not model_path.exists():
        raise FileNotFoundError(f"File not found: {model_path}")
    if model_path.suffix.lower() != ".onnx":
        raise ValueError("Provided file is not an ONNX model")
    
    print(f"Loading ONNX model: {model_path}")

    try:
        model = onnx.load(model_path)
    except Exception as e:
        raise RuntimeError(f"Failed to load ONNX model: {e}")
    
    graph = model.graph
    print(f"Using onnx model {model_path}")

   
    found_node_names = set()
    for node in graph.node:
        found_node_names.update(node.output)

    found_node_names.update(o.name for o in graph.output)
    found_node_names.update(v.name for v in graph.value_info)

    print(f"Found nodes: {found_node_names}")

    logit_tensor_names = []
    for i in range(3):
        name = input(f"Enter branch {i} node name: ").strip()
        logit_tensor_names.append(name)

    print(f"Requested nodes: {logit_tensor_names}")

    for i, name in enumerate(logit_tensor_names):
        if name not in found_node_names:
            raise ValueError(f"Tensor '{name}' does not exist in the model graph")
        
        new_name = f"branch_{i}_logit"
        if new_name in found_node_names:
            raise ValueError(f"Ouput name '{new_name}' already exists in graph")
        
        identity_node = helper.make_node("Identity", inputs=[name], outputs=[new_name], name=f"Rename_{i}")
        graph.node.append(identity_node)
        graph.output.append(helper.make_tensor_value_info(new_name, TensorProto.FLOAT, []))

    modified_path = model_path.with_name(f"{model_path.stem}_MODIFIED{model_path.suffix}")

    try:
        onnx.save(model, modified_path)
    except Exception as e:
        raise RuntimeError(f"Failed to save modified model: {e}")
    
    print(f"Save modified file to {modified_path}")

except KeyboardInterrupt:
    print("\nOperation cancelled by user.")
    sys.exit(1)

except Exception as e:
    print(f"Error: {e}")
    sys.exit(1)