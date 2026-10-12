Imports System
Imports System.Collections.Generic
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DType = org.nd4j.graph.DType
Imports FlatArray = org.nd4j.graph.FlatArray
Imports FlatNode = org.nd4j.graph.FlatNode
Imports FlatProperties = org.nd4j.graph.FlatProperties
Imports IntPair = org.nd4j.graph.IntPair
Imports OpType = org.nd4j.graph.OpType
Imports VarType = org.nd4j.graph.VarType
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.api.ops
Imports Type = org.nd4j.linalg.api.ops.Op.Type
Imports Enter = org.nd4j.linalg.api.ops.impl.controlflow.compat.Enter
Imports [Exit] = org.nd4j.linalg.api.ops.impl.controlflow.compat.Exit
Imports Merge = org.nd4j.linalg.api.ops.impl.controlflow.compat.Merge
Imports NextIteration = org.nd4j.linalg.api.ops.impl.controlflow.compat.NextIteration
Imports Switch = org.nd4j.linalg.api.ops.impl.controlflow.compat.Switch
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ND4UnresolvedOutputVariables = org.nd4j.linalg.exception.ND4UnresolvedOutputVariables
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.autodiff.samediff.serde


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class FlatBuffersMapper
	Public Class FlatBuffersMapper

		Private Sub New()
		End Sub

		''' <summary>
		''' This method converts enums for DataType
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static byte getDataTypeAsByte(@NonNull org.nd4j.linalg.api.buffer.DataType type)
		Public Shared Function getDataTypeAsByte(ByVal type As DataType) As SByte
			Select Case type.innerEnumValue
				Case DataType.InnerEnum.FLOAT
					Return DType.FLOAT
				Case DataType.InnerEnum.DOUBLE
					Return DType.DOUBLE
				Case DataType.InnerEnum.HALF
					Return DType.HALF
				Case DataType.InnerEnum.INT
					Return DType.INT32
				Case DataType.InnerEnum.LONG
					Return DType.INT64
				Case DataType.InnerEnum.BOOL
					Return DType.BOOL
				Case DataType.InnerEnum.SHORT
					Return DType.INT16
				Case DataType.InnerEnum.BYTE
					Return DType.INT8
				Case DataType.InnerEnum.UBYTE
					Return DType.UINT8
				Case DataType.InnerEnum.UTF8
					Return DType.UTF8
				Case DataType.InnerEnum.UINT16
					Return DType.UINT16
				Case DataType.InnerEnum.UINT32
					Return DType.UINT32
				Case DataType.InnerEnum.UINT64
					Return DType.UINT64
				Case DataType.InnerEnum.BFLOAT16
					Return DType.BFLOAT16
				Case Else
					Throw New ND4JIllegalStateException("Unknown or unsupported DataType used: [" & type & "]")
			End Select
		End Function

		''' <summary>
		''' This method converts enums for DataType
		''' </summary>
		Public Shared Function getDataTypeFromByte(ByVal val As SByte) As DataType
			If val = DType.FLOAT Then
				Return DataType.FLOAT
			ElseIf val = DType.DOUBLE Then
				Return DataType.DOUBLE
			ElseIf val = DType.HALF Then
				Return DataType.HALF
			ElseIf val = DType.INT32 Then
				Return DataType.INT
			ElseIf val = DType.INT64 Then
				Return DataType.LONG
			ElseIf val = DType.INT8 Then
				Return DataType.BYTE
			ElseIf val = DType.BOOL Then
				Return DataType.BOOL
			ElseIf val = DType.UINT8 Then
				Return DataType.UBYTE
			ElseIf val = DType.INT16 Then
				Return DataType.SHORT
			ElseIf val = DType.UTF8 Then
				Return DataType.UTF8
			ElseIf val = DType.UINT16 Then
				Return DataType.UINT16
			ElseIf val = DType.UINT32 Then
				Return DataType.UINT32
			ElseIf val = DType.UINT64 Then
				Return DataType.UINT64
			ElseIf val = DType.BFLOAT16 Then
				Return DataType.BFLOAT16
			Else
				Throw New Exception("Unknown datatype: " & val)
			End If
		End Function


		''' <summary>
		''' This method return operation ID for given op name/type pair.
		''' </summary>
		Public Shared Function getOpNum(ByVal name As String, ByVal type As Op.Type) As Long
			If type = Op.Type.LOOP Then
				Return 0
			ElseIf type = Op.Type.RETURN Then
				Return 40
			ElseIf type = Op.Type.CONDITIONAL Then
				Return 10
			ElseIf type = Op.Type.LOOP_COND Then
				Return 70L
			ElseIf type = Type.LOGIC Then
				Select Case name
					Case Enter.OP_NAME
						Return Enter.OP_NUM
					Case [Exit].OP_NAME
						Return [Exit].OP_NUM
					Case NextIteration.OP_NAME
						Return NextIteration.OP_NUM
					Case Merge.OP_NAME
						Return Merge.OP_NUM
					Case Switch.OP_NAME
						Return Switch.OP_NUM
					Case ExternalErrorsFunction.OP_NAME
						Return 0
					Case Else
						Throw New System.InvalidOperationException("Unknown LOGIC op with name: " & name)
				End Select
			ElseIf type = Op.Type.CUSTOM Then
				Dim name2 As val = Nd4j.Executioner.getCustomOperations()(name.ToLower())
				If name2 Is Nothing Then
					Dim name3 As val = Nd4j.Executioner.getCustomOperations()(name)
					If name3 Is Nothing Then
						Return 0
					Else
						Return name3.getHash()
					End If
				Else
					Return name2.getHash()
				End If
				'return Nd4j.getExecutioner().getCustomOperations().get(name.toLowerCase()).getHash();

			Else
				Try
					Dim op As DifferentialFunction = DifferentialFunctionClassHolder.Instance.getInstance(name)
					Return op.opNum()
				Catch e As Exception
					Throw New Exception("Could not find op number for operation: [" & name & "]", e)
				End Try
			End If
		End Function


		''' <summary>
		''' This method converts enums for Op.Type
		''' </summary>
		''' <param name="type"> Byte representing the op type </param>
		''' <returns> Op type </returns>
		Public Shared Function getTypeFromByte(ByVal type As SByte) As Op.Type
			Select Case type
				Case OpType.SCALAR
					Return Op.Type.SCALAR
				Case OpType.SCALAR_BOOL
					Return Op.Type.SCALAR_BOOL
				Case OpType.BROADCAST
					Return Op.Type.BROADCAST
				Case OpType.BROADCAST_BOOL
					Return Op.Type.BROADCAST_BOOL
				Case OpType.TRANSFORM_BOOL
					Return Op.Type.TRANSFORM_BOOL
				Case OpType.TRANSFORM_FLOAT
					Return Op.Type.TRANSFORM_FLOAT
				Case OpType.TRANSFORM_SAME
					Return Op.Type.TRANSFORM_SAME
				Case OpType.TRANSFORM_ANY
					Return Op.Type.TRANSFORM_ANY
				Case OpType.TRANSFORM_STRICT
					Return Op.Type.TRANSFORM_STRICT
				Case OpType.REDUCE_BOOL
					Return Op.Type.REDUCE_BOOL
				Case OpType.REDUCE_LONG
					Return Op.Type.REDUCE_LONG
				Case OpType.REDUCE_FLOAT
					Return Op.Type.REDUCE_FLOAT
				Case OpType.REDUCE_SAME
					Return Op.Type.REDUCE_SAME
				Case OpType.REDUCE_3
					Return Op.Type.REDUCE3
				Case OpType.INDEX_REDUCE
					Return Op.Type.INDEXREDUCE
				Case OpType.RANDOM
					Return Op.Type.RANDOM
				Case OpType.LOGIC
					Return Type.LOGIC
				Case OpType.CUSTOM
					Return Op.Type.CUSTOM
				Case OpType.PAIRWISE
					Return Op.Type.PAIRWISE
				Case OpType.PAIRWISE_BOOL
					Return Op.Type.PAIRWISE_BOOL
				Case OpType.SUMMARYSTATS
					Return Op.Type.SUMMARYSTATS
				Case Else
					Throw New System.NotSupportedException("Unknown op type passed in: " & type)
			End Select
		End Function

		''' <summary>
		''' This method converts an Op.Type to it's corresponding byte value
		''' </summary>
		''' <param name="type"> type to convert </param>
		''' <returns> Byte representing the op type </returns>
		Public Shared Function getFlatOpType(ByVal type As Op.Type) As SByte
			Select Case type
				Case Type.SCALAR
					Return OpType.SCALAR
				Case Type.SCALAR_BOOL
					Return OpType.SCALAR_BOOL
				Case Type.BROADCAST
					Return OpType.BROADCAST
				Case Type.BROADCAST_BOOL
					Return OpType.BROADCAST_BOOL
				Case Type.TRANSFORM_BOOL
					Return OpType.TRANSFORM_BOOL
				Case Type.TRANSFORM_FLOAT
					Return OpType.TRANSFORM_FLOAT
				Case Type.TRANSFORM_SAME
					Return OpType.TRANSFORM_SAME
				Case Type.TRANSFORM_ANY
					Return OpType.TRANSFORM_ANY
				Case Type.TRANSFORM_STRICT
					Return OpType.TRANSFORM_STRICT
				Case Type.SPECIAL
					Return OpType.TRANSFORM_STRICT
				Case Type.REDUCE_FLOAT
					Return OpType.REDUCE_FLOAT
				Case Type.REDUCE_BOOL
					Return OpType.REDUCE_BOOL
				Case Type.REDUCE_SAME
					Return OpType.REDUCE_SAME
				Case Type.REDUCE_LONG
					Return OpType.REDUCE_LONG
				Case Type.REDUCE3
					Return OpType.REDUCE_3
				Case Type.INDEXREDUCE
					Return OpType.INDEX_REDUCE
				Case Type.RANDOM
					Return OpType.RANDOM
				Case Type.CONDITIONAL, [LOOP], [RETURN], LOOP_COND, LOGIC
					Return OpType.LOGIC
				Case Type.CUSTOM
					Return OpType.CUSTOM
				Case Type.PAIRWISE
					Return OpType.PAIRWISE
				Case Type.PAIRWISE_BOOL
					Return OpType.PAIRWISE_BOOL
				Case Type.SUMMARYSTATS, VARIANCE
					Return OpType.SUMMARYSTATS
				Case Else
					Throw New System.NotSupportedException("Unknown op type passed in: " & type)
			End Select
		End Function


		''' <summary>
		''' This method just converts enums
		''' </summary>
		Public Shared Function getOrderFromByte(ByVal val As SByte) As ByteOrder
			If val = org.nd4j.graph.ByteOrder.LE Then
				Return ByteOrder.LITTLE_ENDIAN
			Else
				Return ByteOrder.BIG_ENDIAN
			End If
		End Function

		''' <summary>
		''' This method returns current byte order for this JVM as libnd4j enum
		''' </summary>
		Public Shared ReadOnly Property OrderAsByte As SByte
			Get
				If ByteOrder.nativeOrder().Equals(ByteOrder.BIG_ENDIAN) Then
					Return org.nd4j.graph.ByteOrder.BE
				Else
					Return org.nd4j.graph.ByteOrder.LE
				End If
			End Get
		End Property

		Public Shared Function fromFlatNode(ByVal fn As FlatNode) As DifferentialFunction

			Dim id As Integer = fn.id() 'ID of the node
			Dim name As String = fn.name() 'Name of the node, NOT the name of the op
			Dim opType As Op.Type = FlatBuffersMapper.getTypeFromByte(fn.opType())
			Dim opNum As Long = fn.opNum() 'Op num: hash for custom, number for legacy
			Dim input(fn.inputLength() - 1) As Integer
			For i As Integer = 0 To input.Length - 1
				input(i) = fn.input(i)
			Next i
			Dim inputPaired(fn.inputPairedLength() - 1) As IntPair
			For i As Integer = 0 To inputPaired.Length - 1
				inputPaired(i) = fn.inputPaired(i)
			Next i
			Dim output(fn.outputLength() - 1) As Integer
			For i As Integer = 0 To output.Length - 1
				output(i) = fn.output(i)
			Next i
			Dim extraParams(fn.extraParamsLength() - 1) As Double
			For i As Integer = 0 To extraParams.Length - 1
				extraParams(i) = fn.extraParams(i)
			Next i
			Dim extraInteger(fn.extraIntegerLength() - 1) As Long
			For i As Integer = 0 To extraInteger.Length - 1
				extraInteger(i) = fn.extraInteger(i)
			Next i
			Dim extraBools(fn.extraBoolsLength() - 1) As Boolean
			For i As Integer = 0 To extraBools.Length - 1
				extraBools(i) = fn.extraBools(i)
			Next i
			Dim extraDTypes(fn.extraTypesLength() - 1) As DataType
			For i As Integer = 0 To extraDTypes.Length - 1
				extraDTypes(i) = DataType.fromInt(fn.extraTypes(i))
			Next i

			Dim extraStrings(fn.extraStringsLength() - 1) As String
			For i As Integer = 0 To extraStrings.Length - 1
				extraStrings(i) = fn.extraStrings(i)
			Next i

			Dim dimensions(fn.dimensionsLength() - 1) As Integer
			For i As Integer = 0 To dimensions.Length - 1
				dimensions(i) = fn.dimensions(i)
			Next i
			Dim fa As FlatArray = fn.scalar()
			Dim scalar As INDArray = Nothing
			If fa IsNot Nothing Then
				scalar = Nd4j.createFromFlatArray(fa)
			End If

			Dim flatProperties(fn.propertiesLength() - 1) As FlatProperties
			For i As Integer = 0 To flatProperties.Length - 1
				flatProperties(i) = fn.properties(i)
			Next i
			Dim props As IDictionary(Of String, Object) = FlatBuffersMapper.mapFlatPropertiesToFunctionProperties(java.util.Arrays.asList(flatProperties))

			If opType = Op.Type.CUSTOM OrElse opType = Type.LOGIC Then
				Dim opName As String = fn.opName()

				Dim op As DifferentialFunction
				Dim c As Type = DifferentialFunctionClassHolder.Instance.customOpClassForHashAndName(opNum, opName)

				Preconditions.checkNotNull(c, "Could not find class for hash %s", opNum)

				Try
					op = CType(System.Activator.CreateInstance(c), DifferentialFunction)
				Catch e As Exception When TypeOf e Is IllegalAccessException OrElse TypeOf e Is InstantiationException
					Throw New Exception("Error creating differential function instance of type " & c)
				End Try

				op.setOwnName(name)

				'Set input SDVariables:

				'Set args:
				'op.addTArgument();
				DirectCast(op, CustomOp).addIArgument(extraInteger)
				DirectCast(op, CustomOp).addTArgument(extraParams)
				DirectCast(op, CustomOp).addBArgument(extraBools)
				DirectCast(op, CustomOp).addDArgument(extraDTypes)
				DirectCast(op, CustomOp).addSArgument(extraStrings)

				op.PropertiesForFunction = props
				Return op
			Else
				Dim c As Type = LegacyOpMapper.getLegacyOpClassForId(opType, CInt(opNum))
				Dim op As Op
				Try
					op = DirectCast(System.Activator.CreateInstance(c), Op)
				Catch e As Exception When TypeOf e Is IllegalAccessException OrElse TypeOf e Is InstantiationException
					Throw New Exception("Error creating differential function (Op) instance of type " & c)
				End Try

				If extraParams.Length > 0 Then
					'Assume that extraParams length 0 means extraArgs was originally null, NOT originally length 0
					Dim extraParamsObj(extraParams.Length - 1) As Object
					For i As Integer = 0 To extraParams.Length - 1
						extraParamsObj(i) = extraParams(i)
					Next i
					op.ExtraArgs = extraParamsObj
				End If
				If opType = Op.Type.SCALAR OrElse opType = Op.Type.SCALAR_BOOL Then
					Dim sOp As ScalarOp = DirectCast(op, ScalarOp)
					sOp.setScalar(scalar)
				ElseIf opType = Op.Type.REDUCE_FLOAT OrElse opType = Op.Type.REDUCE3 OrElse opType = Op.Type.SUMMARYSTATS OrElse opType = Op.Type.VARIANCE OrElse opType = Op.Type.REDUCE_BOOL OrElse opType = Op.Type.REDUCE_LONG OrElse opType = Op.Type.REDUCE_SAME Then
					Dim ba As val = DirectCast(op, BaseReduceOp) 'Reduce3 ops are also all BaseAccumulations
					ba.setDimensions(dimensions)
					ba.setDimensionz(Shape.ndArrayDimFromInt(dimensions))
					If extraBools.Length > 0 Then
						ba.setKeepDims(extraBools(0))
					End If

				ElseIf opType = Op.Type.INDEXREDUCE Then
					Dim bia As BaseIndexAccumulation = DirectCast(op, BaseIndexAccumulation)
					bia.setDimensions(dimensions)
					bia.setDimensionz(Shape.ndArrayDimFromInt(dimensions))
					If extraBools.Length > 0 Then
						bia.setKeepDims(extraBools(0))
					End If
				End If
	'            
	'            Op types that don't need any extra/special mapping:
	'            TRANSFORM_BOOL - BooleanNot, IsFinite, IsInf, IsNaN, MatchConditionTransorm
	'            TRANSFORM_ANY - IsMax, Assign
	'            TRANSFORM_FLOAT - Histogram, Sqrt
	'            TRANSFORM_STRICT - Cos, Log, Sigmoid, etc
	'            TRANSFORM_SAME - Abs, Ceil, etc
	'             

				DirectCast(op, DifferentialFunction).PropertiesForFunction = props
				Return DirectCast(op, DifferentialFunction)
			End If
		End Function

		Private Shared ReadOnly EMPTY_BOOLEAN(-1) As Boolean
		Private Shared ReadOnly EMPTY_INT(-1) As Integer
		Private Shared ReadOnly EMPTY_LONG(-1) As Long
		Private Shared ReadOnly EMPTY_DOUBLE(-1) As Double

		Public Shared Function mapFunctionPropertiesToFlatProperties(ByVal fbb As FlatBufferBuilder, ByVal fnProps As IDictionary(Of String, Object)) As Integer()

			Dim outIdxs(fnProps.Count - 1) As Integer
			Dim count As Integer = 0
			For Each e As KeyValuePair(Of String, Object) In fnProps.SetOfKeyValuePairs()
				'Possible types here: primitives (as Number objects), primitive arrays, Strings, String arrays, multi-dimensional string/primitives
				Dim v As Object = e.Value
				Dim iname As Integer = fbb.createString(e.Key)

				Dim i() As Integer = Nothing
				Dim l() As Long = Nothing
				Dim d() As Double = Nothing
				Dim aIdx() As Integer = Nothing
				Dim b() As Boolean = Nothing
				Dim sIdx() As Integer = Nothing
				Dim shape() As Integer = Nothing

				If v Is Nothing Then
					'No op
				ElseIf TypeOf v Is Boolean? Then
					b = New Boolean(){DirectCast(v, Boolean?)}
				ElseIf TypeOf v Is Char? Then
					i = New Integer(){DirectCast(v, Char?)}
				ElseIf TypeOf v Is Number Then
					If TypeOf v Is Double? Then
						d = New Double(){DirectCast(v, Double?)}
					ElseIf TypeOf v Is Single? Then
						d = New Double(){DirectCast(v, Single?)}
					ElseIf TypeOf v Is Integer? Then
						i = New Integer(){DirectCast(v, Integer?)}
					ElseIf TypeOf v Is Long? Then
						l = New Long(){DirectCast(v, Long?)}
					Else
						Throw New System.NotSupportedException("Unable to map property """ & e.Key & """ of type " & v.GetType())
					End If
				ElseIf TypeOf v Is String Then
					Dim str As String = DirectCast(v, String)
					Dim strOffset As Integer = fbb.createString(str)
					sIdx = New Integer(){strOffset}
				ElseIf TypeOf v Is DataType Then
					Dim str As String = v.ToString()
					Dim strOffset As Integer = fbb.createString(str)
					sIdx = New Integer(){strOffset}
				ElseIf TypeOf v Is [Enum] Then
					Dim str As String = v.ToString()
					Dim strOffset As Integer = fbb.createString(str)
					sIdx = New Integer(){strOffset}
				ElseIf TypeOf v Is INDArray Then
					Dim arr As INDArray = DirectCast(v, INDArray)
					aIdx = New Integer(){arr.toFlatArray(fbb)}
				ElseIf v.GetType().IsArray Then
					If v.GetType().GetElementType().isPrimitive() Then
						If TypeOf v Is Boolean() Then
							b = DirectCast(v, Boolean())
							shape = New Integer(){b.Length}
						ElseIf TypeOf v Is Double() Then
							d = DirectCast(v, Double())
							shape = New Integer(){d.Length}
						ElseIf TypeOf v Is Integer() Then
							i = DirectCast(v, Integer())
							shape = New Integer(){i.Length}
						ElseIf TypeOf v Is Long() Then
							l = DirectCast(v, Long())
							shape = New Integer(){l.Length}
						Else
							Throw New System.NotSupportedException("Unable to map property """ & e.Key & """ of type " & v.GetType())
						End If
					ElseIf TypeOf v Is String() Then
						'String[]
						Dim strArr() As String = DirectCast(v, String())
						sIdx = New Integer(strArr.Length - 1){}
						For j As Integer = 0 To strArr.Length - 1
							sIdx(j) = fbb.createString(strArr(j))
						Next j
						shape = New Integer(){strArr.Length}
					ElseIf TypeOf v Is INDArray() Then
						Dim arrArr() As INDArray = DirectCast(v, INDArray())
						aIdx = New Integer(arrArr.Length - 1){}
						For j As Integer = 0 To arrArr.Length - 1
							aIdx(j) = arrArr(j).toFlatArray(fbb)
						Next j
					ElseIf v.GetType().GetElementType().isArray() Then
						shape = ArrayUtil.arrayShape(v, True)
						'Multi-dimensional array
						If TypeOf v Is Boolean()() Then
							b = ArrayUtil.flatten(DirectCast(v, Boolean()()))
						ElseIf TypeOf v Is Boolean()()() Then
							b = ArrayUtil.flatten(DirectCast(v, Boolean()()()))
						ElseIf TypeOf v Is Double()() Then
							d = ArrayUtil.flatten(DirectCast(v, Double()()))
						ElseIf TypeOf v Is Double()()() Then
							d = ArrayUtil.flatten(DirectCast(v, Double()()()))
						ElseIf TypeOf v Is Integer()() Then
							i = ArrayUtil.flatten(DirectCast(v, Integer()()))
						ElseIf TypeOf v Is Integer()()() Then
							i = ArrayUtil.flatten(DirectCast(v, Integer()()()))
						ElseIf TypeOf v Is Long()() Then
							l = ArrayUtil.flatten(DirectCast(v, Long()()))
						ElseIf TypeOf v Is Long()()() Then
							l = ArrayUtil.flatten(DirectCast(v, Long()()()))
						Else
							Throw New System.NotSupportedException("Unable to map multidimensional array property """ & e.Key & """ of type " & v.GetType())
						End If
					End If
				End If

				Dim idxD As Integer = FlatProperties.createDVector(fbb,If(d IsNot Nothing, d, EMPTY_DOUBLE))
				Dim idxI As Integer = FlatProperties.createIVector(fbb,If(i IsNot Nothing, i, EMPTY_INT))
				Dim idxL As Integer = FlatProperties.createLVector(fbb,If(l IsNot Nothing, l, EMPTY_LONG))
				Dim idxA As Integer = FlatProperties.createAVector(fbb,If(aIdx IsNot Nothing, aIdx, EMPTY_INT))
				Dim idxB As Integer = FlatProperties.createBVector(fbb,If(b IsNot Nothing, b, EMPTY_BOOLEAN))
				Dim idxS As Integer = FlatProperties.createSVector(fbb,If(sIdx IsNot Nothing, sIdx, EMPTY_INT))
				Dim idxShape As Integer = FlatProperties.createShapeVector(fbb,If(shape IsNot Nothing, shape, EMPTY_INT))

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: outIdxs[count++] = org.nd4j.graph.FlatProperties.createFlatProperties(fbb, iname, idxI, idxL, idxD, idxA, idxB, idxS, idxShape);
				outIdxs(count) = FlatProperties.createFlatProperties(fbb, iname, idxI, idxL, idxD, idxA, idxB, idxS, idxShape)
					count += 1
			Next e
			Return outIdxs
		End Function

		Public Shared Function mapFlatPropertiesToFunctionProperties(ByVal list As IEnumerable(Of FlatProperties)) As IDictionary(Of String, Object)
			Dim [out] As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			For Each p As FlatProperties In list

				Dim name As String = p.name()
				'Work out type:
				If p.shapeLength() > 0 Then
					'Array type
					Dim shape(p.shapeLength() - 1) As Integer
					For i As Integer = 0 To shape.Length - 1
						shape(i) = p.shape(i)
					Next i
	'                if(shape.length != 1){
	'
	'                    throw new IllegalStateException("Multi-dimensional arrays not yet implemented");
	'                }

					If p.iLength() > 0 Then
						Dim iArr(p.iLength() - 1) As Integer
						For i As Integer = 0 To iArr.Length - 1
							iArr(i) = p.i(i)
						Next i
						If shape.Length = 0 OrElse shape.Length = 1 Then
							[out](name) = iArr
						ElseIf shape.Length = 2 Then
							[out](name) = ArrayUtil.reshapeInt(iArr, shape(0), shape(1))
						ElseIf shape.Length = 3 Then
							[out](name) = ArrayUtil.reshapeInt(iArr, shape(0), shape(1), shape(2))
						End If
					ElseIf p.dLength() > 0 Then
						Dim dArr(p.dLength() - 1) As Double
						For i As Integer = 0 To dArr.Length - 1
							dArr(i) = p.d(i)
						Next i
						If shape.Length = 0 OrElse shape.Length = 1 Then
							[out](name) = dArr
						ElseIf shape.Length = 2 Then
							[out](name) = ArrayUtil.reshapeDouble(dArr, shape(0), shape(1))
						ElseIf shape.Length = 3 Then
							[out](name) = ArrayUtil.reshapeDouble(dArr, shape(0), shape(1), shape(2))
						End If
					ElseIf p.lLength() > 0 Then
						Dim lArr(p.lLength() - 1) As Long
						For i As Integer = 0 To lArr.Length - 1
							lArr(i) = p.l(i)
						Next i
						If shape.Length = 0 OrElse shape.Length = 1 Then
							[out](name) = lArr
						ElseIf shape.Length = 2 Then
							[out](name) = ArrayUtil.reshapeLong(lArr, shape(0), shape(1))
						ElseIf shape.Length = 3 Then
							[out](name) = ArrayUtil.reshapeLong(lArr, shape(0), shape(1), shape(2))
						End If
					ElseIf p.bLength() > 0 Then
						Dim bArr(p.bLength() - 1) As Boolean
						For i As Integer = 0 To bArr.Length - 1
							bArr(i) = p.b(i)
						Next i
						If shape.Length = 0 OrElse shape.Length = 1 Then
							[out](name) = bArr
						ElseIf shape.Length = 2 Then
							[out](name) = ArrayUtil.reshapeBoolean(bArr, shape(0), shape(1))
						ElseIf shape.Length = 3 Then
							[out](name) = ArrayUtil.reshapeBoolean(bArr, shape(0), shape(1), shape(2))
						End If
					ElseIf p.sLength() > 0 Then
						Dim sArr(p.sLength() - 1) As String
						For i As Integer = 0 To sArr.Length - 1
							sArr(i) = p.s(i)
						Next i
						If shape.Length = 0 OrElse shape.Length = 1 Then
							[out](name) = sArr
						ElseIf shape.Length = 2 Then
							[out](name) = ArrayUtil.reshapeObject(sArr, shape(0), shape(1))
						ElseIf shape.Length = 3 Then
							[out](name) = ArrayUtil.reshapeObject(sArr, shape(0), shape(1), shape(2))
						End If
					ElseIf p.aLength() > 0 Then
						Dim iArr(p.aLength() - 1) As INDArray
						For i As Integer = 0 To iArr.Length - 1
							Dim fa As FlatArray = p.a(0)
							iArr(i) = Nd4j.createFromFlatArray(fa)
						Next i
						If shape.Length = 0 OrElse shape.Length = 1 Then
							[out](name) = iArr
						ElseIf shape.Length = 2 Then
							[out](name) = ArrayUtil.reshapeObject(iArr, shape(0), shape(1))
						ElseIf shape.Length = 3 Then
							[out](name) = ArrayUtil.reshapeObject(iArr, shape(0), shape(1), shape(2))
						End If
					Else
						'null property case
						[out](name) = Nothing
					End If
				Else
					'non-array primitive, String or INDArray
					If p.bLength() > 0 Then
						[out](name) = p.b(0)
					ElseIf p.iLength() > 0 Then
						[out](name) = p.i(0)
					ElseIf p.lLength() > 0 Then
						[out](name) = p.l(0)
					ElseIf p.dLength() > 0 Then
						[out](name) = p.d(0)
					ElseIf p.sLength() > 0 Then
						[out](name) = p.s(0)
					ElseIf p.aLength() > 0 Then
						Dim fa As FlatArray = p.a(0)
						[out](name) = Nd4j.createFromFlatArray(fa)
					Else
						'null property case
						[out](name) = Nothing
					End If
				End If
			Next p
			Return [out]
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static int asFlatNode(@NonNull SameDiff sameDiff, @NonNull DifferentialFunction node, @NonNull FlatBufferBuilder bufferBuilder, List<org.nd4j.autodiff.samediff.SDVariable> variables, Map<String, Integer> reverseMap, Map<String, Integer> forwardMap, Map<String, Integer> framesMap, java.util.concurrent.atomic.AtomicInteger idCounter, System.Nullable<Integer> id)
		Public Shared Function asFlatNode(ByVal sameDiff As SameDiff, ByVal node As DifferentialFunction, ByVal bufferBuilder As FlatBufferBuilder, ByVal variables As IList(Of SDVariable), ByVal reverseMap As IDictionary(Of String, Integer), ByVal forwardMap As IDictionary(Of String, Integer), ByVal framesMap As IDictionary(Of String, Integer), ByVal idCounter As AtomicInteger, ByVal id As Integer?) As Integer
			Dim opName As val = node.opName()
			Dim hash As val = FlatBuffersMapper.getOpNum(node.opName(), node.opType())
			'log.info("Exporting node: [{}:<{}> ; OpType: {}; Hash/opNum: {}]", node.opName(), node.tensorflowName(), node.opType(), hash);

			Dim extras() As Double
			If node.opType() = Op.Type.CUSTOM Then
				Dim op As CustomOp = DirectCast(node, CustomOp)
				extras = op.tArgs()
			Else
				Dim eArgs() As Object = node.getExtraArgs()
				extras = If(eArgs IsNot Nothing, New Double(eArgs.Length - 1){}, New Double(){})
				For e As Integer = 0 To extras.Length - 1
					extras(e) = DirectCast(eArgs(e), Number).doubleValue()
				Next e
			End If

			Dim boolArgs() As Boolean = Nothing
			Dim dtypeArgs() As SByte = Nothing
			Dim extraBits() As Long = Nothing
			Dim extraStringIds() As Integer = Nothing
			Dim sArgs() As String = Nothing
			If node.opType() = Op.Type.CUSTOM Then
'JAVA TO VB CONVERTER NOTE: The variable dynamicCustomOp was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim dynamicCustomOp_Conflict As val = CType(node, DynamicCustomOp)
				extraBits = dynamicCustomOp_Conflict.iArgs()
				boolArgs = dynamicCustomOp_Conflict.bArgs()

				If dynamicCustomOp_Conflict.numDArguments() > 0 Then
					dtypeArgs = New SByte(dynamicCustomOp_Conflict.numDArguments() - 1){}
					Dim d As val = dynamicCustomOp_Conflict.dArgs()
					For e As Integer = 0 To dtypeArgs.Length - 1
						dtypeArgs(e) = CSByte(Math.Truncate(d(e).toInt()))
					Next e
				End If

				If dynamicCustomOp_Conflict.numSArguments() > 0 Then
					sArgs = dynamicCustomOp_Conflict.sArgs()
					extraStringIds = New Integer(dynamicCustomOp_Conflict.numSArguments() - 1){}
					For i As Integer = 0 To sArgs.Length - 1
						extraStringIds(i) = bufferBuilder.createString(sArgs(i))
					Next i
				End If

			ElseIf TypeOf node Is Enter Then
				' in case of Enter node we'll be storing unique frame reference
				Dim frameName As val = CType(node, Enter).FrameName
				If Not framesMap.ContainsKey(frameName) Then
					framesMap(frameName) = idCounter.incrementAndGet()
				End If

				extraBits = New Long(){framesMap(frameName).intValue()}
				'keep old extra bits for compatibility, but use extra string ids like the dynamic ops support instead
				sArgs = New String(0){}
				extraStringIds = New Integer(0){}
				sArgs(0) = frameName
				extraStringIds(0) = bufferBuilder.createString(sArgs(0))


			Else
				extraBits = New Long(){}
			End If

			If node.opType() = Op.Type.REDUCE_BOOL OrElse node.opType() = Op.Type.REDUCE_SAME OrElse node.opType() = Op.Type.REDUCE_FLOAT OrElse node.opType() = Op.Type.REDUCE_LONG Then
				Dim op As val = DirectCast(node, ReduceOp)

				boolArgs = New Boolean(1){}
				boolArgs(0) = op.isKeepDims()
				boolArgs(1) = True ' always new format
			ElseIf node.opType() = Op.Type.INDEXREDUCE Then
				Dim op As val = DirectCast(node, IndexAccumulation)

				boolArgs = New Boolean(1){}
				boolArgs(0) = op.isKeepDims()
				boolArgs(1) = True ' always new format
			End If

			Dim inPaired As val = New List(Of Integer)()

			Dim outputIds() As Integer = Nothing
			Dim outputVertexId() As SDVariable = Nothing

			Try
				outputVertexId = node.outputVariables()
				outputIds = New Integer(outputVertexId.Length - 1){}
				For i As Integer = 0 To outputIds.Length - 1
					outputIds(i) = variables.IndexOf(outputVertexId(i))
				Next i
			Catch e As ND4UnresolvedOutputVariables

				outputIds = New Integer(){}
				outputVertexId = Nothing
			Catch e As Exception
				Throw New ND4JIllegalStateException(e)
			End Try


			Dim inputs() As SDVariable = node.args()
			For Each input As SDVariable In inputs
				Dim varName As String = input.name()
				Dim outIdx As Integer
				If sameDiff.getVariables().get(varName).getOutputOfOp() IsNot Nothing Then
					Dim df As DifferentialFunction = sameDiff.getOps().get(sameDiff.getVariables().get(varName).getOutputOfOp()).getOp()
					outIdx = sameDiff.getOps().get(df.getOwnName()).getOutputsOfOp().IndexOf(varName)
				Else
					outIdx = 0
				End If

				If Not reverseMap.ContainsKey(varName) Then
					If varName.Contains("NextIteration") Then
						' forward declaration: Merge node in case of loop will be referring to NextIteration node, which wasn't announced yet
						Dim fwdNodeId As Integer = idCounter.incrementAndGet()
						forwardMap(varName) = fwdNodeId
						reverseMap(varName) = fwdNodeId
					Else
						Throw New ND4JIllegalStateException("Unknown variable used in input: [" & varName & "]")
					End If
				End If

				Dim nodeId As Integer = reverseMap(varName)
				inPaired.add(IntPair.createIntPair(bufferBuilder, nodeId, outIdx))
			Next input

			log.trace("Own Name: {}", node.getOwnName())
			Dim ownId As Integer = If(id IsNot Nothing, id, idCounter.incrementAndGet()) 'forwardMap.containsKey(node.getOwnName()) ? forwardMap.get(node.getOwnName()) : idCounter.incrementAndGet();
			Dim outNames() As String = node.outputVariablesNames()
			For Each s As String In outNames
				If Not reverseMap.ContainsKey(s) Then
					reverseMap(s) = ownId
				End If
			Next s

			Dim dims() As Integer
			Dim t As Type = node.opType()
			If t = Op.Type.REDUCE_FLOAT OrElse t = Op.Type.REDUCE_SAME OrElse t = Op.Type.REDUCE_BOOL OrElse t = Op.Type.REDUCE_LONG OrElse t = Op.Type.INDEXREDUCE OrElse t = Op.Type.REDUCE3 OrElse t = Type.VARIANCE OrElse t = Type.SUMMARYSTATS Then
				dims = node.getDimensions()
				If dims Is Nothing Then
					dims = New Integer(){}
				End If
			Else
				dims = New Integer(){}
			End If

			Dim fnProps As IDictionary(Of String, Object) = node.propertiesForFunction()
			Dim flatProperties() As Integer = FlatBuffersMapper.mapFunctionPropertiesToFlatProperties(bufferBuilder, fnProps)
			Dim propIdx As Integer = FlatNode.createPropertiesVector(bufferBuilder, flatProperties)

			Dim nodesIn As Integer = FlatNode.createInputVector(bufferBuilder, New Integer(){})
			Dim nodesInPaired As Integer = FlatNode.createInputPairedVector(bufferBuilder, Ints.toArray(inPaired))
			Dim nodesOut As Integer = FlatNode.createOutputVector(bufferBuilder, outputIds)
			Dim extraz As Integer = FlatNode.createExtraParamsVector(bufferBuilder, extras)
			Dim integerArgs As Integer = FlatNode.createExtraIntegerVector(bufferBuilder, extraBits)
			Dim bArgs As Integer = FlatNode.createExtraBoolsVector(bufferBuilder,If(boolArgs IsNot Nothing, boolArgs, New Boolean(){}))
			Dim dArgs As Integer = FlatNode.createOutputTypesVector(bufferBuilder,If(dtypeArgs IsNot Nothing, dtypeArgs, New SByte(){}))
			Dim dimensions As Integer = FlatNode.createDimensionsVector(bufferBuilder, dims)
			Dim fname As Integer = bufferBuilder.createString(node.getOwnName())
			Dim scopeName As Integer = bufferBuilder.createString("")
			Dim sArgs3 As Integer = FlatNode.createExtraStringsVector(bufferBuilder,If(extraStringIds IsNot Nothing, extraStringIds, New Integer(){}))
			Dim scalar As Integer = 0
			If TypeOf node Is ScalarOp Then
				Dim sOp As ScalarOp = DirectCast(node, ScalarOp)
				Dim s As INDArray = sOp.scalar()
				If s IsNot Nothing Then
					scalar = s.toFlatArray(bufferBuilder)
				End If
			End If


			If node.opType() Is Nothing Then
				log.warn("Null-op node: {}", node)
			End If


			Dim outVarNames As IList(Of String) = node.getSameDiff().getOps().get(node.getOwnName()).getOutputsOfOp()
			Dim outVarNamesStringsOffsets(If(outVarNames Is Nothing, 0, outVarNames.Count) - 1) As Integer
			For i As Integer = 0 To outVarNamesStringsOffsets.Length - 1
				outVarNamesStringsOffsets(i) = bufferBuilder.createString(outVarNames(i))
			Next i
			Dim outVarNamesOffset As Integer = FlatNode.createOutputNamesVector(bufferBuilder, outVarNamesStringsOffsets)

			Dim opNameOffset As Integer = bufferBuilder.createString(opName)

			Dim outTypes(outVarNames.Count - 1) As SByte
			Dim i As Integer = 0
			For Each s As String In outVarNames
				Dim v As SDVariable = sameDiff.getVariable(s)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: outTypes[i++] = FlatBuffersMapper.getDataTypeAsByte(v.dataType());
				outTypes(i) = FlatBuffersMapper.getDataTypeAsByte(v.dataType())
					i += 1
			Next s
			Dim outTypesOffset As Integer = FlatNode.createOutputTypesVector(bufferBuilder, outTypes)

			'Control dependencies:
			Dim sdo As SameDiffOp = sameDiff.getOps().get(node.getOwnName())

			Dim opCds As Integer = 0
			Dim opCdsArr() As Integer = mapOrNull(sdo.getControlDeps(), bufferBuilder)
			If opCdsArr IsNot Nothing Then
				opCds = FlatNode.createControlDepsVector(bufferBuilder, opCdsArr)
			End If

			Dim varCds As Integer = 0
			Dim varCdsArr() As Integer = mapOrNull(sdo.getVarControlDeps(), bufferBuilder)
			If varCdsArr IsNot Nothing Then
				varCds = FlatNode.createVarControlDepsVector(bufferBuilder, varCdsArr)
			End If

			Dim cdsFor As Integer = 0
			Dim cdsForArr() As Integer = mapOrNull(sdo.getControlDepFor(), bufferBuilder)
			If cdsForArr IsNot Nothing Then
				cdsFor = FlatNode.createControlDepForVector(bufferBuilder, cdsForArr)
			End If


			Dim flatNode As Integer = FlatNode.createFlatNode(bufferBuilder, ownId, fname, FlatBuffersMapper.getFlatOpType(node.opType()), hash, propIdx, nodesIn, nodesInPaired, nodesOut, extraz, integerArgs, bArgs, dimensions, -1, 0, scopeName, outVarNamesOffset, opNameOffset, outTypesOffset, scalar, opCds, varCds, cdsFor, dArgs, sArgs3)

			Return flatNode
		End Function

		Public Shared Function mapOrNull(ByVal list As IList(Of String), ByVal fbb As FlatBufferBuilder) As Integer()
			If list Is Nothing Then
				Return Nothing
			End If
			Dim [out](list.Count - 1) As Integer
			Dim i As Integer=0
			For Each s As String In list
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[i++] = fbb.createString(s);
				[out](i) = fbb.createString(s)
					i += 1
			Next s
			Return [out]
		End Function

		Public Shared Function cloneViaSerialize(ByVal sd As SameDiff, ByVal df As DifferentialFunction) As DifferentialFunction
			Dim nameToIdxMap As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim count As Integer = 0
			For Each v As Variable In sd.getVariables().values()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: nameToIdxMap.put(v.getName(), count++);
				nameToIdxMap(v.getName()) = count
					count += 1
			Next v
			Return cloneViaSerialize(sd, df, nameToIdxMap)
		End Function

		Public Shared Function cloneViaSerialize(ByVal sd As SameDiff, ByVal df As DifferentialFunction, ByVal nameToIdxMap As IDictionary(Of String, Integer)) As DifferentialFunction
			Dim temp2 As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim temp3 As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim temp4 As New AtomicInteger()

			Dim bufferBuilder As val = New FlatBufferBuilder(1024)
			Dim fn As Integer = FlatBuffersMapper.asFlatNode(sd, df, bufferBuilder, sd.variables(), nameToIdxMap, temp2, temp3, temp4, 0)
			bufferBuilder.finish(fn)
			Dim flatNode As FlatNode = FlatNode.getRootAsFlatNode(bufferBuilder.dataBuffer())
			Dim clone As DifferentialFunction = FlatBuffersMapper.fromFlatNode(flatNode)
			Return clone
		End Function

		Public Shared Function toVarType(ByVal variableType As VariableType) As SByte
			Select Case variableType
				Case VariableType.VARIABLE
					Return VarType.VARIABLE
				Case VariableType.CONSTANT
					Return VarType.CONSTANT
				Case VariableType.ARRAY
					Return VarType.ARRAY
				Case VariableType.PLACEHOLDER
					Return VarType.PLACEHOLDER
				Case Else
					Throw New Exception("Unknown variable type: " & variableType)
			End Select
		End Function

		Public Shared Function fromVarType(ByVal varType As SByte) As VariableType
			Select Case varType
				Case VarType.VARIABLE
					Return VariableType.VARIABLE
				Case VarType.CONSTANT
					Return VariableType.CONSTANT
				Case VarType.ARRAY
					Return VariableType.ARRAY
				Case VarType.PLACEHOLDER
					Return VariableType.PLACEHOLDER
				Case Else
					Throw New System.InvalidOperationException("Unknown VarType byte value:" & varType)
			End Select
		End Function
	End Class

End Namespace