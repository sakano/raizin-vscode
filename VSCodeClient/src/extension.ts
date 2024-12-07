import * as fs from "fs";
import * as path from "path";
import * as vscode from "vscode";
import { LanguageClient, LanguageClientOptions, ServerOptions, TransportKind } from "vscode-languageclient/node";

let client: LanguageClient;
let channel: vscode.OutputChannel;

const languageServerPaths = [
  "RaizinLanguageServer.exe",
  "../LanguageServer/RaizinLanguageServer/bin/Debug/net9.0/RaizinLanguageServer.dll"
]

async function activateLanguageServer(context: vscode.ExtensionContext) {
  let serverModule: string | undefined;
  for (let p of languageServerPaths) {
    p = context.asAbsolutePath(p);
    try {
      await fs.promises.access(p);
      serverModule = p;
      break;
    } catch (err) {
    }
  }

  if (!serverModule) {
    throw new URIError("Failed to find the language server module.");
  }

  let serverOptions: ServerOptions;
  if (path.extname(serverModule) === ".dll") {
    serverOptions = {
      run: { command: "dotnet", args: [serverModule], transport: TransportKind.stdio },
      debug: { command: "dotnet", args: [serverModule, "--debug"], transport: TransportKind.stdio },
    };
  } else {
    serverOptions = {
      run: { command: serverModule, args: [], transport: TransportKind.stdio },
      debug: { command: serverModule, args: ["--debug"], transport: TransportKind.stdio },
    };
  }

  let clientOptions: LanguageClientOptions = {
    documentSelector: ["raizin"],
    synchronize: {
      configurationSection: "raizin-vscode",
    },
    outputChannel: channel,
    traceOutputChannel: channel,
  };

  client = new LanguageClient("raizinLanguageServer", "Raizin Language Server", serverOptions, clientOptions);
  client.start();

  if (context.extensionMode === vscode.ExtensionMode.Development) {
    channel.show();
  }
}

export async function activate(context: vscode.ExtensionContext) {
  if (channel === undefined) {
    channel = vscode.window.createOutputChannel("Raizin Language Server");
  }
  context.subscriptions.push(channel);

  await activateLanguageServer(context);
}

export function deactivate(): Thenable<void> | undefined {
  if (!client) {
    return undefined;
  }
  return client.stop();
}

