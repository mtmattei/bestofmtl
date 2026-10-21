import fs from 'node:fs';
import path from 'node:path';
import { gzipSync, brotliCompressSync } from 'node:zlib';
export function prepareStatic(root) {
  const dist=path.join(root,'dist');
  const manifestPath=path.join(dist,'manifest.webmanifest');
  const generated=JSON.parse(fs.readFileSync(manifestPath,'utf8'));
  const source=JSON.parse(fs.readFileSync(path.join(root,'src/BonjourMtl/Platforms/WebAssembly/manifest.webmanifest'),'utf8'));
  const manifest=JSON.stringify({...generated,...source},null,2)+'\n';
  // Resizetizer can retain metadata from an earlier incremental build.
  // Use the checked-in manifest while preserving generated icon paths.
  const indexPath=path.join(dist,'index.html');
  const index=fs.readFileSync(indexPath,'utf8').replace(/<meta name="theme-color" content="[^"]*"\s*\/?\s*>/g,`<meta name="theme-color" content="${source.theme_color}" />`);
  const framework=path.join(dist,'_framework');
  const runtimes=fs.readdirSync(framework).filter(name=>/^dotnet\.[^.]+\.js$/.test(name));
  const packages=fs.readdirSync(dist).filter(name=>name.startsWith('package_'));
  if(runtimes.length!==1 || packages.length!==1) throw new Error('Expected one clean browser runtime and asset package');
  const configPath=path.join(dist,packages[0],'uno-config.js');
  let config=fs.readFileSync(configPath,'utf8');
  const originalRuntime=config.match(/config\.dotnet_js_filename\s*=\s*"([^"]+)"/)?.[1];
  if(!originalRuntime) throw new Error('Browser runtime entrypoint is missing');
  // Uno 10.0.98 can leave the build fingerprint in a custom publish directory.
  // Bind bootstrap and its offline inventory to the actual published runtime.
  config=config.replaceAll(originalRuntime,runtimes[0]);
  for(const [file,content] of [[manifestPath,manifest],[indexPath,index],[configPath,config]]) {
    fs.writeFileSync(file,content);
    fs.writeFileSync(file+'.gz',gzipSync(content));
    fs.writeFileSync(file+'.br',brotliCompressSync(content));
  }
}
