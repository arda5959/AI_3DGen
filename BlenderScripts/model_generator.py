import bpy
import sys
import json
import logging
import tempfile
import os
from pathlib import Path

# Minimum required Blender version
MINIMUM_VERSION = (3, 7, 0)

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

def check_blender_version():
    """Check if Blender version is compatible."""
    version = bpy.app.version
    if version < MINIMUM_VERSION:
        raise RuntimeError(f"Blender version {version} is too old. Minimum required version is {MINIMUM_VERSION}")
    logger.info(f"Using Blender version {version}")

def setup_scene():
    """Prepare the Blender scene by removing default objects."""
    try:
        # Remove all objects
        bpy.ops.object.select_all(action='SELECT')
        bpy.ops.object.delete()
        
        logger.info("Scene setup completed successfully")
        return True
    except Exception as e:
        logger.error(f"Scene setup error: {str(e)}")
        return False

def generate_model(prompt, output_path):
    """Generate a 3D model based on the given prompt."""
    try:
        # Create a basic cube
        bpy.ops.mesh.primitive_cube_add(location=(0, 0, 0))
        cube = bpy.context.object
        
        # Add a subdivision surface modifier
        mod = cube.modifiers.new(name="Subsurf", type='SUBSURF')
        mod.levels = 2
        mod.render_levels = 2
        
        # Apply the modifier
        bpy.ops.object.modifier_apply(modifier=mod.name)
        
        logger.info("Model generation completed successfully")
        
        # Export the model
        export_model(output_path)
        
        return True
    except Exception as e:
        logger.error(f"Model generation error: {str(e)}")
        return False

def export_model(output_path):
    """Export the generated model to the specified path."""
    try:
        # Create output directory if it doesn't exist
        output_dir = Path(output_path).parent
        output_dir.mkdir(parents=True, exist_ok=True)
        
        # Set export settings for Blender 3.7+
        bpy.ops.export_scene.obj(
            filepath=output_path,
            use_selection=True,
            use_mesh_modifiers=True,
            use_normals=True,
            use_uvs=True,
            use_materials=True,
            use_blen_objects=True,
            group_by_object=True,
            group_by_material=True,
            keep_vertex_order=True,
            global_scale=1.0
        )
        
        logger.info(f"Model exported successfully to {output_path}")
        
    except Exception as e:
        logger.error(f"Error exporting model: {str(e)}")
        raise

def main():
    try:
        # Check Blender version first
        check_blender_version()
        
        # Get arguments
        args = json.loads(sys.argv[-1])
        prompt = args.get('prompt', '')
        output_path = args.get('output_path', '')
        
        if not prompt or not output_path:
            raise ValueError("Prompt and output_path are required")
            
        logger.info(f"Starting model generation with prompt: {prompt}")
        
        # Setup scene
        if not setup_scene():
            sys.exit(1)

        # Generate model
        if not generate_model(prompt, output_path):
            sys.exit(1)

        logger.info("Model generation process completed successfully")
        
    except Exception as e:
        logger.error(f"Fatal error: {str(e)}")
        sys.exit(1)

if __name__ == "__main__":
    main()
