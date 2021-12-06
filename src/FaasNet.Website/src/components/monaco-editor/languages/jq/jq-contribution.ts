declare var monaco: any;

export class JQLanguage {
  public static import(): void {
    const languageName: string = "jq";
    monaco.languages.register({ id: languageName });
    monaco.languages.setLanguageConfiguration(languageName, {
      brackets: [
        ['[', ']']
      ]
    });
    monaco.languages.setMonarchTokensProvider(languageName, {
      tokenizer: {
        root: [[/[{}]/, 'delimiter.bracket'], { include: 'common' }],
        common: [
          [/[A-Z][\w\$]\.*/, 'type.identifier'],
          [/[()\[\]]/, '@brackets'],
        ]
      }
    });
    monaco.languages.registerCompletionItemProvider(languageName, {
      provideCompletionItems: (model: any, position: any) => {
        const j: any = model.json;
        var textUntilPosition = model.getValueInRange({
          startLineNumber: 1,
          startColumn: 1,
          endLineNumber: position.lineNumber,
          endColumn: position.column
        });
        let words = textUntilPosition.split(/\.+/).filter((w : string) => {
          return w !== '';
        });
        const fullPath: string = words.join('.');
        var suggestions = [
          {
            label: 'keys',
            kind: monaco.languages.CompletionItemKind.Text,
            insertText: 'keys',
            documentation: 'Returns its keys in an array.'
          },
          {
            label: '|',
            kind: monaco.languages.CompletionItemKind.Text,
            insertText: '|',
            documentation: 'The | operator combines two filters by feeding the output(s) of the one on the left into the input of the one on the right.'
          },
          {
            label: 'length',
            kind: monaco.languages.CompletionItemKind.Text,
            insertText: 'length',
            documentation: 'Gets the length of various different types of value'
          },
          {
            label: 'has',
            kind: monaco.languages.CompletionItemKind.Keyword,
            insertText: 'has(${1:key})',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: 'The builtin function has returns whether the input object has the given key, or the input array has an element at the given index'
          }
        ];
        if (model.json) {
          const properties = this.getAllProperties(j);
          const filteredProperties = properties.filter((s: string) => {
            return s.startsWith(fullPath);
          });
          filteredProperties.forEach((p) => {
            let str = p.replace('[', '').replace(']', '');
            if (str.charAt(0) === '.') {
              str = str.substring(1);
            }

            suggestions.push({
              label: p,
              kind: monaco.languages.CompletionItemKind.Text,
              insertText: str,
              documentation: ''
            });
          });
        }

        return { suggestions: suggestions };
      }
    });
  }

  private static getAllProperties(json: any): string[] {
    let result: string[] = [];
    for (var key in json) {
      const records = JQLanguage.getProperties(key, json, null);
      records.forEach((r: string) => {
        result.push(r);
      });
    }

    return result;
  }

  private static getProperties(key: string, json: any, path: string | null): string[] {
    var result: string[] = [];
    const val: any = json[key];
    const isArray = JQLanguage.isArray(json);
    if (!isArray) {
      if (!path) {
        path = key;
      } else {
        path = path + '.' + key;
      }

      result.push(path);
    } else {
      path = "[]";
    }

    if (typeof val == 'object') {
      for (var k in val) {
        const properties = this.getProperties(k, val[k], path);
        properties.forEach((p: string) => {
          result.push(p);
        });
      }
    }

    return result;
  }

  public static isArray(arr: any) {
    if (!Array.isArray(arr)) {
      return false;
    }

    let result = false;
    arr.forEach((r: any) => {
      if (typeof r == "object") {
        result = true;
      }
    });

    return result;
  }
}
