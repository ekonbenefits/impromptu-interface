// Boots the WebAssembly build under node/bun and runs its MTP entry point
// with this script's own arguments - same trick as BootstrapTests.Wasm's
// runtests.mjs (dotnet.js detects a non-browser JS host and loads the
// runtime itself; no browser or web server needed).
//
// The one extra: a results file MTP writes (--report-anyunit-json <path>)
// lands in Emscripten's in-memory virtual filesystem, which vanishes
// with the process. After Main returns, copy it to the same path on the
// real disk, so the caller sees exactly what a desktop MTP exe would
// have written. Only that flag is mirrored; anything else MTP writes
// (--report-trx, say) would need the same treatment.
import { writeFileSync, mkdirSync } from 'node:fs';
import { dirname } from 'node:path';
import { dotnet } from './_framework/dotnet.js';

// withMainAssembly: the entry point is AnyUnit.TestingPlatform.WasmEntry's
// async Main, not anything in this (F#) project - see that project's
// csproj for why an F# project cannot host one on wasm. The same line
// works for a C# test project too; it just isn't needed there.
const args = process.argv.slice(2);
const instance = await dotnet
    .withMainAssembly('AnyUnit.TestingPlatform.WasmEntry')
    .withApplicationArguments(...args)
    .create();
process.exitCode = await instance.runMain();

const flag = args.indexOf('--report-anyunit-json');
if (flag >= 0 && args[flag + 1] && !args[flag + 1].startsWith('-')) {
    const path = args[flag + 1];
    try {
        const bytes = instance.Module.FS.readFile(path);
        mkdirSync(dirname(path), { recursive: true });
        writeFileSync(path, bytes);
    } catch (e) {
        console.error(`runtests.mjs: no results file at '${path}' in the virtual filesystem: ${e.message}`);
        process.exitCode = process.exitCode || 1;
    }
}
