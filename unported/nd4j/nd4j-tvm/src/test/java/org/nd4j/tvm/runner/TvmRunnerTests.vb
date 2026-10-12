Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports org.bytedeco.cpython
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.bytedeco.cpython.global.python
Imports org.bytedeco.numpy.global.numpy
import static org.junit.jupiter.api.Assertions.assertEquals
Imports TempDir = org.junit.jupiter.api.io.TempDir

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 
Namespace org.nd4j.tvm.runner



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TvmRunnerTests
	Public Class TvmRunnerTests

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static void PrepareTestLibs(String libPath) throws Exception
		Friend Shared Sub PrepareTestLibs(ByVal libPath As String)
			Py_AddPath(org.bytedeco.tvm.presets.tvm.cachePackages())
			Py_Initialize()
			If _import_array() < 0 Then
				Console.Error.WriteLine("numpy.core.multiarray failed to import")
				PyErr_Print()
				Environment.Exit(-1)
			End If
			Dim globals As PyObject = PyModule_GetDict(PyImport_AddModule("__main__"))

			PyRun_StringFlags("""""""Script to prepare test_relay_add.so""""""" & vbLf & "import tvm" & vbLf & "import numpy as np" & vbLf & "from tvm import relay" & vbLf & "import os" & vbLf & "x = relay.var(""x"", shape=(1, 1), dtype=""float32"")" & vbLf & "y = relay.var(""y"", shape=(1, 1), dtype=""float32"")" & vbLf & "params = {""y"": np.ones((1, 1), dtype=""float32"")}" & vbLf & "mod = tvm.IRModule.from_expr(relay.Function([x, y], x + y))" & vbLf & "# build a module" & vbLf & "compiled_lib = relay.build(mod, tvm.target.create(""llvm""), params=params)" & vbLf & "# export it as a shared library" & vbLf & "dylib_path = os.path.join(""" & libPath & """, ""test_relay_add.so"")" & vbLf & "compiled_lib.export_library(dylib_path)" & vbLf, Py_file_input, globals, globals, Nothing)

			If PyErr_Occurred() IsNot Nothing Then
				Console.Error.WriteLine("Python error occurred")
				PyErr_Print()
				Environment.Exit(-1)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAdd(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAdd(ByVal tempDir As Path)
			' try to use MKL when available 
			System.setProperty("org.bytedeco.openblas.load", "mkl")

			Dim libPath As File = tempDir.resolve("lib").toFile()
			PrepareTestLibs(libPath.getAbsolutePath().replace(Path.DirectorySeparatorChar, "/"c))
			Dim f As New File(libPath, "test_relay_add.so")
			Dim x As INDArray = Nd4j.scalar(1.0f).reshape(ChrW(1), 1)
			Dim tvmRunner As TvmRunner = TvmRunner.builder().modelUri(f.getAbsolutePath()).build()
			Dim inputs As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			inputs("x") = x
			Dim exec As IDictionary(Of String, INDArray) = tvmRunner.exec(inputs)
			Dim z As INDArray = exec("0")
			assertEquals(2.0,z.sumNumber().doubleValue(),1e-1)
		End Sub
	End Class

End Namespace