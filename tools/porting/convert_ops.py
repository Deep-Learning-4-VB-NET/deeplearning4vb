"""Mechanical conversion of ND4J op classes (Java-to-VB converter output) to DeepLearning4VB.

Usage (from any directory):
    python convert_ops.py [--report report.txt] FILE.vb [FILE.vb ...]

Each FILE must already be moved (plain mv) from unported/ to its mirrored path under
src/DeepLearning4VB/org/nd4vb/; it is rewritten in place. What the script does:

* Imports: renames org.nd4j -> DeepLearning4VB.org.nd4vb, Nd4j -> Nd4vbT alias, Shape -> ShapeT alias,
  ND4J* -> ND4VB*; drops lombok/java/shade/commons imports; puts every alias import into the fence
  (#If SAMEDIFF / MODEL_IMPORT / FLATBUFFERS) of the code that uses it, or drops it when unused.
* Members: constructors and methods whose signature uses SameDiff/SDVariable go inside #If SAMEDIFF;
  members using TensorFlow/ONNX types, initFromTensorFlow/initFromOnnx and the attribute mappings
  (mappingsForFunction, attributeAdaptersForFunction) inside #If MODEL_IMPORT; FlatBuffers members inside
  #If FLATBUFFERS. A method whose signature is plain but whose body needs SameDiff (args(), arg(),
  sameDiff, ...) is fenced with an #Else branch that throws NotSupportedException. Fenced text is the
  converter text, unchanged.
* Unfenced code: tabs -> 4 spaces; renamed fields (x_Conflict -> xField, extraArgs -> extraArgsField,
  dimensions -> dimensionsField, keepDims -> keepDimsField, ...); Java Number -> Object; Op.Type enum
  references; the converter's OpType property -> getOpType() function; condition.getValue() -> .Value;
  "Dim x As val =" -> "Dim x ="; converter TODO comments dropped; a public parameterless constructor is
  added for Lombok's @NoArgsConstructor; a class summary is generated when the class has none.
* Prints REVIEW lines for what still needs a hand: Java collection calls, Lombok @Data, unresolved
  imports, ...

The custom ops (DynamicCustomOp subclasses) go through the same script; the renames for the
DynamicCustomOp fields (inputArguments_Conflict -> inputArgumentsField, ...) are included.
"""
import pathlib
import re
import sys

ROOT = pathlib.Path(__file__).resolve().parents[2]  # the repository root (tools/porting/../..)
SRC = ROOT / "src" / "DeepLearning4VB"
# Types that exist only as stubs in wave 2 (ported by another agent or by wave 3).
KNOWN_TYPES = {
    "DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb",
    "DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape",
    "DeepLearning4VB.org.nd4vb.linalg.util.LinAlgExceptions",
    "DeepLearning4VB.org.nd4vb.linalg.factory.Broadcast",
    "DeepLearning4VB.org.nd4vb.linalg.ops.transforms.Transforms",
}

SD_SIG = re.compile(r"\b(SameDiff|SDVariable|SameDiffUtils|SDIndex|SDValue|InferenceSession)\b")
MI_SIG = re.compile(r"\b(NodeDef|AttrValue|GraphDef|OpDef|Onnx\.\w+|TFGraphMapper|OnnxGraphMapper|"
                    r"TensorflowDescriptorParser|OnnxDescriptorParser|DifferentialFunctionClassHolder)\b")
FB_SIG = re.compile(r"\b(FlatBufferBuilder|FlatBuffersMapper|FlatArray|FlatNode|FlatVariable)\b")
MI_NAMES = {"initFromTensorFlow", "initFromOnnx", "mappingsForFunction", "attributeAdaptersForFunction"}
SD_NAMES = {"doDiff", "outputVariables", "outputVariable", "outputs", "args", "arg", "larg", "rarg", "diff"}
SD_BODY = re.compile(r"\bsameDiff\b|\bSDVariable\b|\bSameDiff\b|\bSameDiffUtils\b|(?<![\w.])(l|r)?arg\(\)|"
                     r"(?<![\w.])args\(\)|(?<![\w.])arg\(\d+\)|(?<![\w.])outputVariables?\(|(?<![\w.])f\(\)\.|"
                     r"\bsd\.\w|\bMe\.(l|r)?args?\(")

MEMBER_START = re.compile(
    r"^\s*(<[^>]*>\s*)?((Public|Private|Protected|Friend|Shared|Overrides|Overridable|MustOverride|NotOverridable|"
    r"Overloads|Shadows|ReadOnly|WriteOnly|Default|Partial|MustInherit|NotInheritable|Async|Iterator|Widening|"
    r"Narrowing)\s+)*(Sub|Function|Property|Class|Structure|Interface|Enum|Module|Event|Operator)\b(?!\s*\()")
LAMBDA_START = re.compile(r"(?<![\w.])(Function|Sub)\s*\([^()]*\)\s*(As\s+[\w.()]+)?\s*$")
END_RE = re.compile(r"^\s*End\s+(Sub|Function|Property|Class|Structure|Interface|Enum|Module|Operator|Get|Set)\b")

LICENSE_MARK = "'@LICENSE@"


def kind_of(sig):
    m = MEMBER_START.match(sig)
    return m.group(4) if m else None


def member_name(sig):
    m = re.search(r"\b(?:Sub|Function|Property|Class|Structure|Interface|Enum|Module)\s+(\[?\w+\]?)", sig)
    return m.group(1).strip("[]") if m else None


def is_single_line(sig, following):
    k = kind_of(sig)
    if k is None:
        return True
    if re.search(r"\bMustOverride\b", sig) or k == "Event":
        return True
    if k == "Property":
        nxt = next((l.strip() for l in following if l.strip() and not l.strip().startswith("'")), "")
        return not (nxt.startswith("Get") or nxt.startswith("Set") or re.match(r"^(Private |Protected |Friend )?(Get|Set)\b", nxt))
    return False


class Item:
    def __init__(self, comments, lines, kind, name):
        self.comments = comments
        self.lines = lines
        self.kind = kind          # Sub/Function/Property/Class/... or "Field" or "Blank"
        self.name = name
        self.fence = None         # None, "SAMEDIFF", "MODEL_IMPORT", "FLATBUFFERS"
        self.fallback = None      # list of lines for the #Else branch

    @property
    def sig(self):
        return self.lines[0] if self.lines else ""

    @property
    def body(self):
        return "\n".join(self.lines[1:])

    @property
    def text(self):
        return "\n".join(self.lines)


def parse_class_body(lines):
    """Split the lines between the class header and End Class into items."""
    items = []
    pending = []
    i = 0
    n = len(lines)
    while i < n:
        line = lines[i]
        s = line.strip()
        if not s:
            if pending:
                pending.append(line)
            else:
                items.append(Item([], [line], "Blank", None))
            i += 1
            continue
        if s.startswith("'") or re.match(r"^<[^>]*>$", s):
            pending.append(line)
            i += 1
            continue
        k = kind_of(line)
        if k is None:
            items.append(Item(pending, [line], "Field", None))
            pending = []
            i += 1
            continue
        if is_single_line(line, lines[i + 1:i + 4]):
            items.append(Item(pending, [line], k, member_name(line)))
            pending = []
            i += 1
            continue
        # block member
        depth = 0
        j = i
        block = []
        while j < n:
            l = lines[j]
            if j == i or (MEMBER_START.match(l) and not is_single_line(l, lines[j + 1:j + 4])):
                depth += 1
            elif LAMBDA_START.search(l.split("'")[0]):
                depth += 1
            if re.match(r"^\s*End\s+(Sub|Function|Property|Class|Structure|Interface|Enum|Module|Operator)\b", l):
                depth -= 1
            block.append(l)
            j += 1
            if depth == 0:
                break
        items.append(Item(pending, block, k, member_name(line)))
        pending = []
        i = j
    if pending:
        items.append(Item(pending, [], "Blank", None))
    return items


def classify(item, report, fname):
    if item.kind in ("Blank",):
        return
    sig = item.sig
    text = item.text
    if item.kind == "Field":
        if SD_SIG.search(sig):
            item.fence = "SAMEDIFF"
        elif MI_SIG.search(sig):
            item.fence = "MODEL_IMPORT"
        return
    name = item.name
    if SD_SIG.search(sig) or (name in SD_NAMES and item.kind in ("Function", "Sub")):
        item.fence = "SAMEDIFF"
        return
    if MI_SIG.search(sig) or name in MI_NAMES:
        item.fence = "MODEL_IMPORT"
        return
    if FB_SIG.search(sig):
        item.fence = "FLATBUFFERS"
        return
    body = "\n".join(l for l in item.lines[1:] if not l.strip().startswith("'"))
    for fence, rx, why in (("SAMEDIFF", SD_BODY, "SameDiff (phase 2 milestone M5)"),
                           ("MODEL_IMPORT", MI_SIG, "model import (phase 3, samediff-import)"),
                           ("FLATBUFFERS", FB_SIG, "FlatBuffers serialization (phase 2 milestone M5)")):
        if rx.search(body):
            item.fence = fence
            if name == "New" or item.kind not in ("Function", "Sub", "Property"):
                report.append(f"REVIEW {fname}: constructor/member {name} uses {fence} in its body; fenced without fallback")
            elif item.kind == "Property":
                report.append(f"REVIEW {fname}: property {name} uses {fence}; fenced without fallback - write one")
            else:
                newsig = convert_line(sig.split(" Implements ")[0].rstrip())
                end = "End Function" if item.kind == "Function" else "End Sub"
                item.fallback = [newsig,
                                 f'            Throw New NotSupportedException("{name} needs {why}")',
                                 f"        {end}"]
            return


RENAMES = [
    (r"\bx_Conflict\b", "xField"), (r"\by_Conflict\b", "yField"), (r"\bz_Conflict\b", "zField"),
    (r"\bkeepDims_Conflict\b", "keepDimsField"), (r"\bdimension_Conflict\b", "dimensionField"),
    (r"\binputArguments_Conflict\b", "inputArgumentsField"), (r"\boutputArguments_Conflict\b", "outputArgumentsField"),
    (r"\boutputVariables_Conflict\b", "outputVariablesField"), (r"\bopName_Conflict\b", "opNameField"),
    (r"\bbiasCorrected_Conflict\b", "biasCorrectedField"),
    (r"\bMe\.extraArgs\b(?!\s*\()", "Me.extraArgsField"), (r"^(\s*)extraArgs(\s*=)", r"\1extraArgsField\2"),
    (r"\bMe\.dimensions\b(?!\s*\()", "Me.dimensionsField"),
    (r"\bMe\.keepDims\b", "Me.keepDimsField"),
    (r"\bMe\.isComplex\b", "Me.isComplexField"), (r"\bMe\.isEmptyReduce\b", "Me.isEmptyReduceField"),
    (r"\bND4JIllegalStateException\b", "ND4VBIllegalStateException"),
    (r"\bND4JIllegalArgumentException\b", "ND4VBIllegalArgumentException"),
    (r"\bND4JException\b", "ND4VBException"),
    (r"(?<![\w.])Nd4j\.", "Nd4vbT."), (r"(?<![\w.])Shape\.", "ShapeT."),
    (r"(?<![\w.])Transforms\.", "TransformsT."), (r"(?<![\w.])FastMath\.", "Math."),
    (r"\bNew LinkedHashMap\(Of\b", "New JavaCompat.LinkedHashMap(Of"),
    (r"\bAs Number\b", "As Object"), (r"\bAs Number\(\)", "As Object()"),
    (r"\b([cC]ondition\w*)\.getValue\(\)", r"\1.Value"),
    (r"\bDim (\w+) As val =", r"Dim \1 ="), (r"\bFor Each (\w+) As val In\b", r"For Each \1 In"),
    (r"(Function (?:opType|getOpType)\(\)) As Type\b", r"\1 As Op.Type"),
    (r"(?<![\w.])Type\.([A-Z][A-Z_0-9]+)\b", r"Op.Type.\1"),
    (r"\s+Implements org\.nd4j\.[\w.]+", ""),
]


def convert_line(line):
    m = re.match(r"^(\t*)(.*)$", line)
    out = "    " * len(m.group(1)) + m.group(2)
    for a, b in RENAMES:
        out = re.sub(a, b, out)
    st = out.strip()
    if not st.startswith("'"):
        # Java identifiers that are VB keywords (lower case; the VB keywords themselves are capitalized), outside
        # string literals and comments
        parts = re.split(r'("(?:[^"]|"")*")', out)
        for k in range(0, len(parts), 2):
            seg = parts[k]
            cut = seg.find("'")
            code, comment = (seg, "") if cut < 0 else (seg[:cut], seg[cut:])
            code = KEYWORD_ID.sub(lambda mm: "[" + mm.group(1) + "]", code)
            parts[k] = code + comment
            if cut >= 0:
                parts = parts[:k + 1]
                break
        out = "".join(parts)
    return out


KEYWORD_ID = re.compile(r"(?<![\w.\[])\b(set|get|step|when|end|loop|next|error|stop|call|module|alias|let|rem)\b(?![\w\]])")


def convert_member(item):
    """Converted text lines of an unfenced item."""
    out = []
    for c in item.comments:
        s = c.strip()
        if s.startswith("'JAVA TO VB CONVERTER") or s.startswith("'ORIGINAL LINE"):
            continue
        out.append(convert_line(c))
    lines = item.lines
    # the converter's OpType property -> getOpType() function
    if item.kind == "Property" and item.name == "OpType":
        sig = convert_line(lines[0])
        mods = re.match(r"^(\s*)(.*?)\bReadOnly Property OpType As (Op\.)?Type\b(.*)$", sig)
        body = []
        inside = False
        for l in lines[1:]:
            st = l.strip()
            if st.startswith("Get"):
                inside = True
                continue
            if st.startswith("End Get"):
                inside = False
                continue
            if inside:
                body.append(convert_line(l[1:] if l.startswith("\t") else l))
        mod = mods.group(2).replace("Overridable ", "Overridable Overloads ") if "Overrides" not in mods.group(2) else mods.group(2)
        out.append(f"{mods.group(1)}{mod}Function getOpType() As Op.Type")
        out.extend(body)
        out.append(f"{mods.group(1)}End Function")
        return out
    elem = {"calculateOutputDataTypes": "DataType", "calculateOutputShape": "LongShapeDescriptor"}.get(item.name)
    for l in lines:
        s = l.strip()
        if s.startswith("'JAVA TO VB CONVERTER") or s.startswith("'ORIGINAL LINE"):
            continue
        c = convert_line(l)
        if elem:
            c = re.sub(r"\bCollections\.singletonList\((.*)\)\s*$", lambda m: f"New List(Of {elem}) From {{{m.group(1)}}}", c)
            c = re.sub(r"\b(java\.util\.)?Collections\.emptyList\(\)", f"New List(Of {elem})()", c)
        out.append(c)
    return out


VALUE_TYPES = {"Boolean", "Integer", "Long", "Double", "Single", "Short", "Byte", "Char", "SByte", "UInteger",
               "ULong", "UShort", "Decimal", "DataType", "Date"}


def ctor_params(sig):
    """[(type, is_paramarray)] of an unfenced 'Sub New(...)' line; array types end with '()'."""
    m = re.search(r"Sub New\((.*)\)\s*$", sig)
    if not m or not m.group(1).strip():
        return [] if m else None
    res = []
    for p in split_args(m.group(1)):
        pm = re.match(r"^\s*(Optional\s+)?(ParamArray\s+)?(ByVal\s+|ByRef\s+)?\[?(\w+)\]?(\(\))?\s+As\s+([\w.()]+(?:\(Of [^)]*\))?)", p)
        if not pm:
            return None
        typ = pm.group(6) + ("()" if pm.group(5) else "")
        res.append((typ, bool(pm.group(2))))
    return res


def split_args(s):
    out, depth, cur, instr = [], 0, "", False
    for ch in s:
        if ch == '"':
            instr = not instr
        if not instr:
            if ch in "({":
                depth += 1
            elif ch in ")}":
                depth -= 1
            elif ch == "," and depth == 0:
                out.append(cur)
                cur = ""
                continue
        cur += ch
    if cur.strip():
        out.append(cur)
    return out


def candidates_at(sigs, nargs, pos):
    types = set()
    for ps in sigs:
        if ps is None:
            continue
        if len(ps) == nargs:
            types.add(ps[pos][0])
        if ps and ps[-1][1] and nargs >= len(ps) - 1 and nargs != len(ps):
            # ParamArray in its expanded form
            types.add(ps[pos][0] if pos < len(ps) - 1 else ps[-1][0][:-2] if ps[-1][0].endswith("()") else ps[-1][0])
    return types


def cast_nothing_args(lines, own_sigs, base_sigs):
    """Java's null never matches a primitive, VB's Nothing does: cast Nothing arguments of Me.New/MyBase.New."""
    out = []
    for l in lines:
        m = re.match(r"^(\s*)(Me|MyBase)\.New\((.*)\)\s*$", l)
        if m and "Nothing" in m.group(3):
            args = split_args(m.group(3))
            sigs = own_sigs if m.group(2) == "Me" else base_sigs
            changed = False
            for i, a in enumerate(args):
                if a.strip() != "Nothing":
                    continue
                types = candidates_at(sigs, len(args), i)
                refs = {t for t in types if t.rstrip("()") not in VALUE_TYPES or t.endswith("()")}
                if len(types) > 1 and len(refs) == 1:
                    args[i] = (" " if a.startswith(" ") else "") + f"CType(Nothing, {next(iter(refs))})"
                    changed = True
            if changed:
                l = f"{m.group(1)}{m.group(2)}.New({', '.join(x.strip() for x in args)})"
        out.append(l)
    return out


def base_ctor_sigs(inherits, imports, fname):
    """Unfenced constructor signatures of the base class (already converted or hand-written)."""
    target = None
    for imp in imports:
        m = re.match(rf"^Imports\s+{re.escape(inherits)}\s*=\s*org\.nd4j\.([\w.]+)$", imp.strip().lstrip("﻿"))
        if m:
            target = SRC / "org" / "nd4vb" / (m.group(1).replace(".", "/") + ".vb")
    if target is None:
        target = (SRC / fname).parent / (inherits + ".vb")
    if not target.exists():
        return []
    sigs = []
    fence = 0
    for l in target.read_text(encoding="utf-8-sig").split("\n"):
        s = l.strip()
        if s.startswith("#If "):
            fence += 1
        elif s.startswith("#End If"):
            fence -= 1
        elif fence == 0 and re.match(r"^(Public|Protected|Friend|Protected Friend)\s+Sub New\(", s):
            sigs.append(ctor_params(s))
    return sigs


def fenced_text(item):
    return list(item.comments) + list(item.lines)


IMPORT_DROP = re.compile(r"^Imports\s+(\w+\s*=\s*)?(lombok\.|java\.|javax\.|org\.nd4j\.shade\.|org\.apache\.commons\.|"
                         r"com\.google\.|org\.bytedeco\.)")


def convert_import(line):
    """Returns (alias or None, new Imports line or None)."""
    s = line.strip().lstrip("\ufeff")
    if IMPORT_DROP.match(s):
        return None, None
    m = re.match(r"^Imports\s+(\w+)\s*=\s*([\w.]+)$", s)
    if m:
        alias, target = m.group(1), m.group(2)
        if target.startswith("org.nd4j."):
            target = "DeepLearning4VB.org.nd4vb." + target[len("org.nd4j."):]
            target = re.sub(r"\bND4J", "ND4VB", target)
            alias = re.sub(r"\bND4J", "ND4VB", alias)
            if target.endswith(".factory.Nd4j"):
                target, alias = target[:-len("Nd4j")] + "Nd4vb", "Nd4vbT"
            if target.endswith(".api.shape.Shape") and alias == "Shape":
                alias = "ShapeT"
            if target.endswith(".ops.transforms.Transforms") and alias == "Transforms":
                alias = "TransformsT"
        return alias, f"Imports {alias} = {target}"
    m = re.match(r"^Imports\s+([\w.]+)$", s)
    if m:
        target = m.group(1)
        if target.startswith("org.nd4j."):
            target = "DeepLearning4VB.org.nd4vb." + target[len("org.nd4j."):]
        return None, f"Imports {target}"
    return None, s


def target_exists(target):
    if target in KNOWN_TYPES:
        return True
    if not target.startswith("DeepLearning4VB."):
        return target.startswith("System")
    rel = target[len("DeepLearning4VB."):].replace(".", "/")
    if (SRC / (rel + ".vb")).exists():
        return True
    # nested type or member import: check the parent
    parent = rel.rsplit("/", 1)[0]
    return (SRC / (parent + ".vb")).exists()


def namespace_exists(target):
    if not target.startswith("DeepLearning4VB."):
        return True
    rel = target[len("DeepLearning4VB."):].replace(".", "/")
    return (SRC / rel).is_dir()


def convert_file(path, report):
    raw = path.read_text(encoding="utf-8-sig").replace("\r\n", "\n")
    lines = raw.split("\n")
    try:
        fname = str(path.relative_to(SRC)).replace("\\", "/")
    except ValueError:  # dry run outside src/: use the part from "org/nd4vb/" on
        s = str(path).replace("\\", "/")
        fname = s[s.index("org/nd4vb/"):] if "org/nd4vb/" in s else path.name

    imports, header, rest = [], [], []
    stage = 0
    for l in lines:
        s = l.strip().lstrip("\ufeff")
        if stage == 0 and s.startswith("Imports "):
            imports.append(s)
            continue
        if s.startswith("Namespace "):
            stage = 2
        if stage < 2:
            header.append(l)
        else:
            rest.append(l)
    while header and not header[0].strip():
        header.pop(0)
    while header and not header[-1].strip():
        header.pop()

    ns_line = rest[0].replace("Namespace org.nd4j.", "Namespace org.nd4vb.")
    # class header: find the first Class line
    ci = next(i for i, l in enumerate(rest) if re.match(r"^\s*(<[^>]*>\s*)?(Public |Friend )?(MustInherit |NotInheritable )?Class\b", l))
    pre_class = rest[1:ci]
    # class declaration lines: Class, Inherits, Implements
    cj = ci + 1
    while cj < len(rest) and re.match(r"^\s*(Inherits|Implements)\b", rest[cj]):
        cj += 1
    class_decl = rest[ci:cj]
    # End Class / End Namespace from the end
    ei = max(i for i, l in enumerate(rest) if re.match(r"^\s*End Class\b", l))
    body = rest[cj:ei]
    items = parse_class_body(body)
    # the converter's abstract "ExtraArgs" property (Lombok's setExtraArgs, implemented by BaseOp) goes away
    items = [it for it in items if not (it.kind == "Property" and it.name == "ExtraArgs" and "MustOverride" in it.sig)]

    class_name = member_name(class_decl[0])
    inherits = next((l.strip().split()[1] for l in class_decl if l.strip().startswith("Inherits")), "")

    lombok = " ".join(l for l in pre_class if "ORIGINAL LINE" in l)
    for ann in ("@Data", "@Getter", "@Setter", "@AllArgsConstructor", "@Builder", "@EqualsAndHashCode", "@ToString"):
        if ann in lombok:
            report.append(f"REVIEW {fname}: class annotation {ann} - write the Lombok members by hand")
    for it in items:
        for c in it.comments:
            if "ORIGINAL LINE: @" in c and re.search(r"@(Getter|Setter|Builder)", c):
                report.append(f"REVIEW {fname}: member annotation in {c.strip()[:90]}")

    for it in items:
        classify(it, report, fname)

    # opName for the summary
    op_name = None
    for it in items:
        if it.name == "opName" and it.kind == "Function":
            m = re.search(r'Return\s+"([^"]*)"', it.text)
            if m:
                op_name = m.group(1)
    if op_name is None:
        for it in items:
            m = re.search(r'Const OP_NAME As String = "([^"]*)"', it.text)
            if m:
                op_name = m.group(1)

    # output body
    out_items = []
    has_noargs = any(it.name == "New" and re.search(r"Sub New\(\s*\)", it.sig) and it.fence is None for it in items)
    if "@NoArgsConstructor" in lombok and not has_noargs:
        out_items.append(["        Public Sub New()", "        End Sub", ""])
    group_fence = None
    buf = []

    def flush():
        nonlocal buf, group_fence
        if buf:
            out_items.append(buf)
        buf = []

    for it in items:
        if it.kind == "Blank":
            if group_fence is None:
                buf.append("")
            else:
                buf.append("")
            continue
        if it.fence is None:
            if group_fence is not None:
                while buf and not buf[-1].strip():
                    buf.pop()
                buf.append("#End If")
                buf.append("")
                group_fence = None
            buf.extend(convert_member(it))
        elif it.fallback:
            if group_fence is not None:
                while buf and not buf[-1].strip():
                    buf.pop()
                buf.append("#End If")
                buf.append("")
                group_fence = None
            buf.append(f"#If {it.fence} Then")
            buf.extend(fenced_text(it))
            buf.append("#Else")
            buf.extend(it.fallback)
            buf.append("#End If")
        else:
            if group_fence != it.fence:
                if group_fence is not None:
                    while buf and not buf[-1].strip():
                        buf.pop()
                    buf.append("#End If")
                    buf.append("")
                buf.append(f"#If {it.fence} Then")
                group_fence = it.fence
            buf.extend(fenced_text(it))
    if group_fence is not None:
        while buf and not buf[-1].strip():
            buf.pop()
        buf.append("#End If")
    flush()
    body_lines = [l for grp in out_items for l in grp]
    # collapse multiple blank lines
    collapsed = []
    for l in body_lines:
        if not l.strip() and collapsed and not collapsed[-1].strip():
            continue
        collapsed.append(l)
    while collapsed and not collapsed[0].strip():
        collapsed.pop(0)
    while collapsed and not collapsed[-1].strip():
        collapsed.pop()

    # Nothing arguments in constructor chains
    own_sigs = []
    fz = 0
    for l in collapsed:
        s = l.strip()
        if s.startswith("#If "):
            fz += 1
        elif s.startswith("#End If") or s.startswith("#Else"):
            fz = 0 if s.startswith("#End If") else fz
        elif fz == 0 and re.match(r"^(Public|Protected|Friend|Protected Friend)\s+Sub New\(", s):
            own_sigs.append(ctor_params(s))
    out_lines, fz = [], 0
    bsigs = base_ctor_sigs(inherits, imports, fname)
    chunk = []
    for l in collapsed:
        s = l.strip()
        if s.startswith("#"):
            out_lines.extend(cast_nothing_args(chunk, own_sigs, bsigs))
            chunk = []
            fz = 1 if s.startswith("#If ") else 0
            out_lines.append(l)
        elif fz:
            out_lines.append(l)
        else:
            chunk.append(l)
    out_lines.extend(cast_nothing_args(chunk, own_sigs, bsigs))
    collapsed = out_lines

    # per-fence text for import placement
    unf = "\n".join(collapsed)
    fence_text = {"SAMEDIFF": "", "MODEL_IMPORT": "", "FLATBUFFERS": ""}
    cur = None
    plain = []
    for l in collapsed:
        m = re.match(r"^#If (\w+) Then", l)
        if m:
            cur = m.group(1)
            continue
        if l.startswith("#Else"):
            cur = None
            continue
        if l.startswith("#End If"):
            cur = None
            continue
        if cur:
            fence_text[cur] += l + "\n"
        else:
            plain.append(l)
    plain_text = "\n".join(re.sub(r'"[^"]*"', '""', l) for l in plain if not l.strip().startswith("'")) + "\n" + \
        "\n".join(convert_line(l) for l in class_decl)

    # imports
    groups = {None: [], "SAMEDIFF": [], "MODEL_IMPORT": [], "FLATBUFFERS": [], "SD_OR_MI": []}
    for imp in imports:
        alias, new = convert_import(imp)
        if new is None:
            continue
        if alias is None:
            m = re.match(r"^Imports\s+([\w.]+)$", new)
            target = m.group(1) if m else ""
            if target.startswith("System") or namespace_exists(target):
                groups[None].append(new)
            else:
                groups["SD_OR_MI"].append(new)
            continue
        target = new.split("=", 1)[1].strip()
        orig_alias = re.match(r"^Imports\s+(\w+)", imp.strip().lstrip("\ufeff")).group(1)
        used_plain = re.search(rf"(?<![\w.]){re.escape(alias)}\b", plain_text) is not None
        used = {f: re.search(rf"(?<![\w.]){re.escape(orig_alias)}\b", t) is not None for f, t in fence_text.items()}
        # fenced code keeps the converter text, so it refers to the original alias
        fixed = "Imports " + imp.strip().lstrip("\ufeff")[len("Imports "):]
        fixed = fixed.replace("= org.nd4j.", "= DeepLearning4VB.org.nd4vb.")
        fixed = re.sub(r"(= DeepLearning4VB\.org\.nd4vb\.[\w.]*)ND4J", r"\1ND4VB", fixed)
        fixed = re.sub(r"\.factory\.Nd4j$", ".factory.Nd4vb", fixed)
        if used_plain:
            if not target_exists(target):
                report.append(f"REVIEW {fname}: unfenced import of a missing type: {new}")
            groups[None].append(new)
            if alias != orig_alias:
                fl = [f for f, u in used.items() if u]
                if len(fl) == 1:
                    groups[fl[0]].append(fixed)
                elif fl:
                    groups["SD_OR_MI"].append(fixed)
        else:
            fl = [f for f, u in used.items() if u]
            if not fl:
                continue
            if len(fl) == 1:
                groups[fl[0]].append(fixed)
            else:
                groups["SD_OR_MI"].append(fixed)
    if re.search(r"\.(isFPType|isIntType|isNumerical|isSigned|width|toBool)\(", plain_text) and \
            "Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer" not in groups[None]:
        groups[None].append("Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer")
    imp_out = ["Imports System"] if "Imports System" not in groups[None] else []
    imp_out += groups[None]
    for f, cond in (("SAMEDIFF", "SAMEDIFF"), ("MODEL_IMPORT", "MODEL_IMPORT"), ("FLATBUFFERS", "FLATBUFFERS"),
                    ("SD_OR_MI", "SAMEDIFF OrElse MODEL_IMPORT")):
        if groups[f]:
            imp_out.append(f"#If {cond} Then")
            imp_out.extend(groups[f])
            imp_out.append("#End If")

    # class summary
    pre = [l for l in pre_class if not ("JAVA TO VB CONVERTER" in l or "ORIGINAL LINE" in l)]
    pre = [convert_line(l) for l in pre if l.strip()]
    has_doc = any(l.strip().startswith("'''") for l in pre)
    if not has_doc:
        java_name = "org.nd4j." + fname[len("org/nd4vb/"):-3].replace("/", ".")
        what = {"BaseTransformSameOp": "transform same", "BaseTransformFloatOp": "transform float",
                "BaseTransformStrictOp": "transform strict", "BaseTransformBoolOp": "transform bool",
                "BaseTransformAnyOp": "transform any", "BaseTransformOp": "transform", "BaseScalarOp": "scalar",
                "BaseScalarBoolOp": "scalar bool", "BaseBroadcastOp": "broadcast", "BaseBroadcastBoolOp": "broadcast bool",
                "BaseReduceFloatOp": "reduce float", "BaseReduceSameOp": "reduce same", "BaseReduceLongOp": "reduce long",
                "BaseReduceBoolOp": "reduce bool", "BaseReduce3Op": "reduce3", "BaseIndexAccumulation": "index reduce",
                "Variance": "summary stats", "BaseRandomOp": "random", "DynamicCustomOp": "custom"}.get(inherits, None)
        if op_name and what:
            pre = [f"    ''' <summary>The legacy {what} op <c>{op_name}</c> (Java <c>{java_name}</c>).</summary>"] if what != "custom" else \
                  [f"    ''' <summary>The custom op <c>{op_name}</c> (Java <c>{java_name}</c>).</summary>"]
        else:
            pre = [f"    ''' <summary>Java <c>{java_name}</c>.</summary>"]
    decl = [convert_line(l) for l in class_decl]

    out = imp_out + [""] + header + ["", ns_line, ""] + pre + decl + [""] + ["    " + l if False else l for l in collapsed] + \
        ["", "    End Class", "", "End Namespace", ""]
    text = "\n".join(out)
    # review hints for the unfenced code
    for rx, msg in ((r"\bCollections\.\w+", "java.util.Collections call"), (r"\bjava\.", "java.* reference"),
                    (r"\.SequenceEqual\(", "SequenceEqual (null-unsafe)"), (r"\bArrays\.asList\(", "Arrays.asList"),
                    (r"\bLinkedHashMap\b", "LinkedHashMap"), (r"\bval\b", "lombok val"),
                    (r"\.size\(\)", ".size()"), (r"\bMe\.dimensions\b", "dimensions"),
                    (r"(?<![\w.])dimensions(?![\w(])", "bare dimensions (field or parameter?)")):
        for l in plain:
            if re.search(rx, l):
                report.append(f"REVIEW {fname}: {msg}: {l.strip()[:100]}")
                break
    path.write_text(text, encoding="utf-8")
    return class_name


def main(argv):
    report = []
    rep_path = None
    files = []
    it = iter(argv)
    for a in it:
        if a == "--report":
            rep_path = next(it)
        else:
            files.append(pathlib.Path(a))
    for f in files:
        try:
            convert_file(f, report)
        except Exception as e:  # keep going, report the failure
            report.append(f"FAILED {f}: {e!r}")
    text = "\n".join(report)
    if rep_path:
        pathlib.Path(rep_path).write_text(text + "\n", encoding="utf-8")
    print(f"{len(files)} files, {len(report)} report lines")


if __name__ == "__main__":
    main(sys.argv[1:])
