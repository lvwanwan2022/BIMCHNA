import os
import sys

# Add tools to path to import doc_manager
sys.path.append(os.path.dirname(os.path.abspath(__file__)))
import doc_manager

def scan_and_register():
    # Project root is one level up from tools
    project_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    print(f"Scanning project root: {project_root}")
    
    count = 0
    for root, dirs, files in os.walk(project_root):
        # Skip hidden directories and tools directory itself (optional, but good practice if we only want docs)
        # Actually we want docs wherever they are, but usually they are in root or crates/docs
        if '.git' in root or '__pycache__' in root:
            continue
            
        for filename in files:
            if filename.endswith('.md'):
                file_path = os.path.join(root, filename)
                
                # Parse filename to see if it matches our pattern
                info = doc_manager.parse_filename(file_path)
                
                if info and 'id' in info:
                    # Calculate relative path from project root
                    rel_path = os.path.relpath(root, project_root)
                    if rel_path == '.':
                        rel_path = ''
                    
                    # Update info path to be relative
                    info['path'] = rel_path.replace('\\', '/')
                    
                    print(f"Registering: {filename} in {info['path']}")
                    doc_manager.register_document(
                        id=info.get('id'),
                        level=info.get('level'),
                        parent_id=info.get('parent_id'),
                        filename=info.get('filename'),
                        title=info.get('title'),
                        creator=info.get('creator'),
                        path=info.get('path')
                    )
                    count += 1
    
    print(f"Total documents registered: {count}")

if __name__ == "__main__":
    scan_and_register()

