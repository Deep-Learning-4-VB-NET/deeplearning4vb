Imports System
Imports ArrayListMultimap = org.nd4j.shade.guava.collect.ArrayListMultimap
Imports Multimap = org.nd4j.shade.guava.collect.Multimap
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.api.ops
Imports CublasPointer = org.nd4j.linalg.jcublas.CublasPointer
Imports JCudaBuffer = org.nd4j.linalg.jcublas.buffer.JCudaBuffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext

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

Namespace org.nd4j.linalg.jcublas.util


	''' <summary>
	''' Handles conversion of
	''' arguments passed to jcuda
	''' to their proper primitives
	''' when invoked with pointers.
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class CudaArgs
		Private Sub New()
		End Sub

		''' <summary>
		''' For invoking a cuda kernel
		''' this returns the module opName for the given op </summary>
		''' <param name="op"> the op to get the module opName for </param>
		''' <returns> the module opName for the given op </returns>
		Public Shared Function getModuleNameFor(ByVal op As Op) As String
			'String functionName = op instanceof TransformOp || op instanceof ReduceOp || op instanceof IndexAccumulation ? op.opName() + "_strided" : op.opName();
			Dim moduleName As String = Nothing
			If TypeOf op Is ReduceOp Then

				moduleName = "reduce"

				' FIXME: special case for reduce3
				If op.opName().Equals("cosinesimilarity") Then
					moduleName = "reduce3"
				ElseIf op.opName().Equals("euclidean") Then
					moduleName = "reduce3"
				ElseIf op.opName().Equals("manhattan") Then
					moduleName = "reduce3"
				End If

			ElseIf TypeOf op Is TransformOp Then
				' FIXME: we need special case for pairwise transforms for now. Later we should make them separate kernel call
				If op.opName().Equals("add") Then
					moduleName = "pairWiseTransform"
				ElseIf op.opName().Equals("copy") Then
					moduleName = "pairWiseTransform"
				ElseIf op.opName().Equals("div") Then
					moduleName = "pairWiseTransform"
				ElseIf op.opName().Equals("mul") Then
					moduleName = "pairWiseTransform"
				ElseIf op.opName().Equals("rdiv") Then
					moduleName = "pairWiseTransform"
				ElseIf op.opName().Equals("rsub") Then
					moduleName = "pairWiseTransform"
				ElseIf op.opName().Equals("sub") Then
					moduleName = "pairWiseTransform"

				Else
					moduleName = "transform"
				End If
			ElseIf TypeOf op Is ScalarOp Then
				moduleName = "scalar"
			ElseIf TypeOf op Is BroadcastOp Then
				moduleName = "broadcast"
			ElseIf TypeOf op Is IndexAccumulation Then
				moduleName = "indexReduce"
			End If
			Return moduleName
		End Function

		Public Shared Function getOpCode(ByVal op As Op) As Integer
			Dim code As Integer = -1

			Dim name As String = op.opName()

			If TypeOf op Is ReduceOp Then
				If name.Equals("mean") Then
					code = 0
				ElseIf name.Equals("sum") Then
					code = 1
				ElseIf name.Equals("bias") Then
					code = 2
				ElseIf name.Equals("max") Then
					code = 3
				ElseIf name.Equals("min") Then
					code = 4
				ElseIf name.Equals("norm1") Then
					code = 5
				ElseIf name.Equals("norm2") Then
					code = 6
				ElseIf name.Equals("normmax") Then
					code = 7
				ElseIf name.Equals("prod") Then
					code = 8
				ElseIf name.Equals("std") Then
					code = 9
				ElseIf name.Equals("var") Then
					code = 10


					' FIXME: special case for reduce3
				ElseIf name.Equals("manhattan") Then
					code = 0
				ElseIf name.Equals("euclidean") Then
					code = 1
				ElseIf name.Equals("cosinesimilarity") Then
					code = 2
				End If
			ElseIf TypeOf op Is TransformOp Then

				If name.Equals("abs") Then
					code = 0
				ElseIf name.Equals("ceil") Then
					code = 1
				ElseIf name.Equals("cos") Then
					code = 2
				ElseIf name.Equals("exp") Then
					code = 3
				ElseIf name.Equals("floor") Then
					code = 4
				ElseIf name.Equals("log") Then
					code = 5
				ElseIf name.Equals("neg") Then
					code = 6
				ElseIf name.Equals("pow") Then
					code = 7
				ElseIf name.Equals("round") Then
					code = 8
				ElseIf name.Equals("setrange") Then
					code = 9
				ElseIf name.Equals("sigmoid") Then
					code = 10
				ElseIf name.Equals("sign") Then
					code = 11
				ElseIf name.Equals("sin") Then
					code = 12
				ElseIf name.Equals("softplus") Then
					code = 13
				ElseIf name.Equals("sqrt") Then
					code = 14
				ElseIf name.Equals("tanh") Then
					code = 15
				ElseIf name.Equals("acos") Then
					code = 16
				ElseIf name.Equals("asin") Then
					code = 17
				ElseIf name.Equals("atan") Then
					code = 18

					' FIXME: we need special case for pairwise transforms for now. Later we should make them separate kernel call
				ElseIf name.Equals("add") Then
					code = 0
				ElseIf name.Equals("copy") Then
					code = 1
				ElseIf name.Equals("div") Then
					code = 2
				ElseIf name.Equals("eq") Then
					code = 3
				ElseIf name.Equals("gt") Then
					code = 4
				ElseIf name.Equals("lt") Then
					code = 5
				ElseIf name.Equals("mul") Then
					code = 6
				ElseIf name.Equals("rdiv") Then
					code = 7
				ElseIf name.Equals("rsub") Then
					code = 8
				ElseIf name.Equals("sub") Then
					code = 9
				ElseIf name.Equals("eps") Then
					code = 10
				ElseIf name.Equals("gte") Then
					code = 11
				ElseIf name.Equals("lte") Then
					code = 12
				ElseIf name.Equals("max") Then
					code = 13
				ElseIf name.Equals("min") Then
					code = 14
				ElseIf name.Equals("neq") Then
					code = 15
				End If

			ElseIf TypeOf op Is ScalarOp Then
				If name.StartsWith("add", StringComparison.Ordinal) Then
					code = 0
				ElseIf name.StartsWith("sub", StringComparison.Ordinal) Then
					code = 1
				ElseIf name.StartsWith("mul", StringComparison.Ordinal) Then
					code = 2
				ElseIf name.StartsWith("div", StringComparison.Ordinal) Then
					code = 3
				ElseIf name.StartsWith("rdiv", StringComparison.Ordinal) Then
					code = 4
				ElseIf name.StartsWith("rsub", StringComparison.Ordinal) Then
					code = 5
				ElseIf name.StartsWith("max", StringComparison.Ordinal) Then
					code = 6
				ElseIf name.StartsWith("lessthan", StringComparison.Ordinal) Then
					code = 7
				ElseIf name.StartsWith("greaterthan", StringComparison.Ordinal) Then
					code = 8
				ElseIf name.StartsWith("eq", StringComparison.Ordinal) Then
					code = 9
				ElseIf name.StartsWith("lte", StringComparison.Ordinal) Then
					code = 10
				ElseIf name.StartsWith("neq", StringComparison.Ordinal) Then
					code = 11
				ElseIf name.StartsWith("min", StringComparison.Ordinal) Then
					code = 12
				ElseIf name.StartsWith("set", StringComparison.Ordinal) Then
					code = 13
				End If
			ElseIf TypeOf op Is BroadcastOp Then
				If name.Equals("broadcastadd") Then
					code = 0
				ElseIf name.Equals("broadcastsub") Then
					code = 1
				ElseIf name.Equals("broadcastmul") Then
					code = 2
				ElseIf name.Equals("broadcastdiv") Then
					code = 3
				ElseIf name.Equals("broadcastrdiv") Then
					code = 4
				ElseIf name.Equals("broadcastrsub") Then
					code = 5
				ElseIf name.Equals("broadcastcopy") Then
					code = 6
				End If
			ElseIf TypeOf op Is IndexAccumulation Then
				If name.Equals("imax") Then
					code = 0
				ElseIf name.Equals("imin") Then
					code = 1
				End If
			End If

			' System.out.println("CALLING ["+getModuleNameFor(op)+"] -> ["+code+"]");

			Return code
		End Function


		''' <summary>
		''' Returns number of SMs, based on device compute capability and number of processors.
		''' </summary>
		''' <param name="ccMajor"> </param>
		''' <param name="ccMinor">
		''' @return </param>
		Public Shared Function convertMPtoCores(ByVal ccMajor As Integer, ByVal ccMinor As Integer, ByVal numberOfProcessors As Integer) As Integer
			' Defines for GPU Architecture types (using the SM version to determine the # of cores per SM

			If ccMajor = 1 Then
				Return 8
			End If
			If ccMajor = 2 AndAlso ccMinor = 1 Then
				Return 48
			End If
			If ccMajor = 2 Then
				Return 32
			End If
			If ccMajor = 3 Then
				Return 192
			End If
			If ccMajor = 5 Then
				Return 128
			End If

			' return negative number if device is unknown
			Return -1
		End Function


		''' 
		''' <param name="context"> </param>
		''' <param name="kernelParams">
		''' @return </param>
		Public Shared Function argsAndReference(ByVal context As CudaContext, ParamArray ByVal kernelParams() As Object) As ArgsAndReferences
			'      Map<Object, Object> idMap = new IdentityHashMap<>();
			Dim kernelParameters(kernelParams.Length - 1) As Object
			'        List<CublasPointer> pointersToFree = new ArrayList<>();
			Dim arrayToPointer As Multimap(Of INDArray, CublasPointer) = ArrayListMultimap.create()
			For i As Integer = 0 To kernelParams.Length - 1
				Dim arg As Object = kernelParams(i)

				' If the instance is a JCudaBuffer we should assign it to the device
				If TypeOf arg Is JCudaBuffer Then
					Dim buffer As JCudaBuffer = DirectCast(arg, JCudaBuffer)
					'                if (!idMap.containsKey(buffer)) {
					Dim pointerToFree As New CublasPointer(buffer, context)
					kernelParameters(i) = pointerToFree.DevicePointer
					'                    pointersToFree.add(pointerToFree);
					'                    idMap.put(buffer, pointerToFree.getPointer());
					'                } else {
					'                    Pointer pointer = (Pointer) idMap.get(buffer);
					'                    kernelParameters[i] = pointer;
					'                }

				ElseIf TypeOf arg Is INDArray Then
					Dim array As INDArray = DirectCast(arg, INDArray)
					'array.norm2(0);
					'                if (!idMap.containsKey(array)) {
					Dim pointerToFree As New CublasPointer(array, context)
					kernelParameters(i) = pointerToFree.DevicePointer
					'                    pointersToFree.add(pointerToFree);
					arrayToPointer.put(array, pointerToFree)
					'                    idMap.put(array, pointerToFree.getPointer());
					'                } else {
					'                    Pointer pointer = (Pointer) idMap.get(array);
					'                    kernelParameters[i] = pointer;
					'                }

				Else
					kernelParameters(i) = arg
				End If

			Next i

			Return New ArgsAndReferences(kernelParameters, arrayToPointer)
			'return new ArgsAndReferences(kernelParameters,idMap,pointersToFree,arrayToPointer);
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor public static class ArgsAndReferences
		Public Class ArgsAndReferences
			Friend args() As Object
			'        private Map<Object,Object> idMap;
			'        private List<CublasPointer> pointersToFree;
			''' <summary>
			''' conversion list of arrays to their assigned cublas pointer
			''' </summary>
			Friend arrayToPointer As Multimap(Of INDArray, CublasPointer)


		End Class


	End Class

End Namespace