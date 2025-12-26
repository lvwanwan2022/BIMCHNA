import os
import sys
# 添加当前目录到 sys.path 以便导入 doc_manager
sys.path.append(os.path.dirname(os.path.abspath(__file__)))
import doc_manager

def rebuild():
    print("正在初始化数据库...")
    doc_manager.init_db()
    
    project_root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    print(f"扫描项目根目录: {project_root}")
    
    count = 0
    for root, dirs, files in os.walk(project_root):
        # 忽略隐藏目录和特定目录
        dirs[:] = [d for d in dirs if not d.startswith('.') and d not in ['node_modules', 'terminals']]
        
        for file in files:
            if not file.endswith('.md'):
                continue
                
            # 简单的过滤器，确保是我们的管理文档
            if '_id_' not in file or '_level_' not in file:
                continue
                
            full_path = os.path.join(root, file)
            # 计算相对于项目根目录的路径
            rel_dir = os.path.relpath(root, project_root)
            if rel_dir == '.':
                rel_dir = ''
            
            # 使用 doc_manager 解析文件名
            # 注意：parse_filename 期望传入的是路径，它会自己拆分目录和文件名
            # 但它的逻辑比较简单，我们构造一个假路径传入，或者只传文件名，然后手动修正 path
            
            info = doc_manager.parse_filename(file)
            if info:
                # 覆盖解析出来的 path，使用我们计算的相对路径
                info['path'] = rel_dir
                # 将路径中的反斜杠转换为正斜杠，保证跨平台兼容性
                info['path'] = info['path'].replace('\\', '/')
                
                print(f"注册文档: {info['title']} ({file}) -> {info['path']}")
                doc_manager.register_document(**info)
                count += 1
            else:
                print(f"跳过无法解析的文件: {file}")

    print(f"重构完成，共录入 {count} 个文档。")

if __name__ == '__main__':
    rebuild()

