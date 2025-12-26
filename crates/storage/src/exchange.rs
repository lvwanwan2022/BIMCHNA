use std::path::Path;
use anyhow::Result;

pub trait Importer {
    fn extensions(&self) -> &[&str];
    fn import(&self, path: &Path) -> Result<()>;
}

pub trait Exporter {
    fn extensions(&self) -> &[&str];
    fn export(&self, path: &Path) -> Result<()>;
}

pub struct FbxImporter;

impl Importer for FbxImporter {
    fn extensions(&self) -> &[&str] {
        &["fbx"]
    }

    fn import(&self, path: &Path) -> Result<()> {
        println!("Importing FBX from {:?}", path);
        // Implementation would go here
        Ok(())
    }
}
