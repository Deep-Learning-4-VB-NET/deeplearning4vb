Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public abstract class BaseOp extends org.nd4j.autodiff.functions.DifferentialFunction implements Op
	Public MustInherit Class BaseOp
		Inherits DifferentialFunction
		Implements Op

		Public MustOverride WriteOnly Property ExtraArgs Implements Op.setExtraArgs As Object()

'JAVA TO VB CONVERTER NOTE: The field x was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field y was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field z was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend x_Conflict, y_Conflict, z_Conflict As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected String xVertexId,yVertexId,zVertexId;
		Protected Friend xVertexId, yVertexId, zVertexId As String
		' cached instance, for dataType checks
		Protected Friend extraArgz As DataBuffer

		Protected Friend dimensionz As INDArray

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal inPlace As Boolean, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, inPlace, extraArgs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, extraArgs)
		End Sub

		''' <summary>
		''' Specify an alternative result array
		''' </summary>
		''' <param name="x"> the input </param>
		''' <param name="z"> the output array </param>
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			Me.New(x, Nothing, z)
		End Sub


		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			MyBase.New(False)
			Me.x_Conflict = x
			Me.y_Conflict = y
			Me.z_Conflict = z
		End Sub


		''' <summary>
		''' An op for one ndarray
		''' </summary>
		''' <param name="x"> the ndarray </param>
		Public Sub New(ByVal x As INDArray)
			Me.New(x, Nothing, x)
		End Sub

		Public Shared Function getOpType(ByVal op As Op) As Type
			Dim type As Type = Nothing

			If TypeOf op Is CustomOp Then
				Return Type.CUSTOM
			ElseIf TypeOf op Is TransformOp Then
				If op.y() Is Nothing Then
					type = Type.TRANSFORM_FLOAT
				Else
					type = Op.Type.PAIRWISE
				End If
			ElseIf TypeOf op Is ReduceOp Then
				If op.y() Is Nothing Then
					type = DirectCast(op, ReduceOp).OpType
				Else
					type = Op.Type.REDUCE3
				End If
			ElseIf TypeOf op Is ScalarOp Then
				type = Op.Type.SCALAR
			ElseIf TypeOf op Is BroadcastOp Then
				type = Op.Type.BROADCAST
			ElseIf TypeOf op Is IndexAccumulation Then
				type = Op.Type.INDEXREDUCE
			ElseIf TypeOf op Is MetaOp Then
				type = Type.META
			ElseIf TypeOf op Is GridOp Then
				type = Type.GRID
			End If

			Return type
		End Function



		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
		End Sub

		Public Overridable Function extraArgsDataBuff(ByVal dtype As DataType) As DataBuffer Implements Op.extraArgsDataBuff
			If extraArgz IsNot Nothing Then
				Return extraArgz
			End If

			If extraArgs IsNot Nothing Then
				If Shape.isZ(dtype) OrElse Shape.isB(dtype) Then
					Dim extraz(extraArgs.Length - 1) As Long
					For i As Integer = 0 To extraArgs.Length - 1
						If TypeOf extraArgs(i) Is Number Then
							Dim arg As Number = DirectCast(extraArgs(i), Number)
							Dim val As Long = arg.longValue()
							extraz(i) = val
						End If
					Next i
					extraArgz = Nd4j.ConstantHandler.getConstantBuffer(extraz, dtype)
					Return extraArgz
				ElseIf Shape.isR(dtype) Then
					Dim extraz(extraArgs.Length - 1) As Double
					For i As Integer = 0 To extraArgs.Length - 1
						If Not (TypeOf extraArgs(i) Is Number) Then
							Continue For
						End If
						Dim arg As Number = DirectCast(extraArgs(i), Number)
						If arg Is Nothing Then
							arg = 0.0
						End If
						Dim val As Double = arg.doubleValue()
						extraz(i) = val
					Next i
					extraArgz = Nd4j.ConstantHandler.getConstantBuffer(extraz, dtype)
					Return extraArgz
				End If
			End If

			Return Nothing
		End Function

		Public Overridable Function extraArgsBuff() As Buffer Implements Op.extraArgsBuff
			If extraArgs IsNot Nothing Then
				Dim retBuff As DataBuffer
				If x_Conflict.data().dataType() = DataType.FLOAT Then
					retBuff = Nd4j.createBuffer(New Single(extraArgs.Length - 1){})
					For i As Integer = 0 To extraArgs.Length - 1
						Dim val As Number = DirectCast(extraArgs(i), Number)
						retBuff.put(i, val.floatValue())
					Next i
					Return retBuff.asNioFloat()
				Else
					retBuff = Nd4j.createBuffer(New Double(extraArgs.Length - 1){})
					For i As Integer = 0 To extraArgs.Length - 1
						Dim val As Number = DirectCast(extraArgs(i), Number)
						retBuff.put(i, val.doubleValue())
					Next i
					Return retBuff.asNioDouble()
				End If


			End If
			Return Nothing
		End Function

		Public Overridable WriteOnly Property X Implements Op.setX As INDArray
			Set(ByVal x As INDArray)
				Me.x_Conflict = x
			End Set
		End Property

		Public Overridable WriteOnly Property Z Implements Op.setZ As INDArray
			Set(ByVal z As INDArray)
				Me.z_Conflict = z
			End Set
		End Property

		Public Overridable WriteOnly Property Y Implements Op.setY As INDArray
			Set(ByVal y As INDArray)
				Me.y_Conflict = y
			End Set
		End Property

		Public Overridable Function extraArgs() As Object() Implements Op.extraArgs
			Return extraArgs
		End Function

		Public Overridable Function x() As INDArray Implements Op.x
			Return x_Conflict
		End Function

		Public Overridable Function y() As INDArray Implements Op.y
			Return y_Conflict
		End Function


		Public Overridable Function z() As INDArray Implements Op.z
			Return z_Conflict
		End Function

		Public Overrides Function getInputArgument(ByVal index As Integer) As INDArray
			Preconditions.checkState(index >= 0 AndAlso index < 2, "Input argument index must be 0 or 1, got %s", index)
			Return If(index = 0, x_Conflict, y_Conflict)
		End Function

		Public Overrides Function outputVariables(ByVal baseName As String) As SDVariable()
			If zVertexId Is Nothing Then
				Dim outputNames As val = sameDiff.getOutputsForOp(Me)
				'no need to dynamically create if already exists
				If outputNames IsNot Nothing Then
					zVertexId = sameDiff.getVariable(outputNames(0)).name()


					Return New SDVariable(){sameDiff.getVariable(outputNames(0))}
				End If

				If isInPlace() Then
					Dim newVars As val = sameDiff.generateOutputVariableForOp(Me,Nothing,False)
					Dim inputArr As val = x()
					'in place op
					If inputArr Is Nothing Then
						Return newVars
					End If

					sameDiff.setArrayForVariable(newVars(0).name(),inputArr)
					z_Conflict = inputArr
					If sameDiff.getOutputsForOp(Me) Is Nothing Then
						sameDiff.addOutgoingFor(newVars,Me)
					End If
					Return newVars
				End If

				Dim newVars() As SDVariable = sameDiff.generateOutputVariableForOp(Me, baseName, False)
				If sameDiff.getOutputsForOp(Me) Is Nothing Then
					sameDiff.addOutgoingFor(newVars, Me)
				End If
				Return newVars
			End If

			Return New SDVariable(){sameDiff.getVariable(zVertexId)}
		End Function


		Public Overrides Function ToString() As String
			Return opName()
		End Function


		Public Overridable Function toCustomOp() As CustomOp Implements Op.toCustomOp
			Dim customOpBuilder As DynamicCustomOp.DynamicCustomOpsBuilder = DynamicCustomOp.builder(opName())
			customOpBuilder.callInplace(x() Is z())

			If y() IsNot Nothing Then
				customOpBuilder.addInputs(x(), y())
			Else
				customOpBuilder.addInputs(x())
			End If

			customOpBuilder.addOutputs(z())
			If extraArgs IsNot Nothing Then
				For i As Integer = 0 To extraArgs.Length - 1
					If TypeOf extraArgs(i) Is Integer? Then
						customOpBuilder.addIntegerArguments(DirectCast(extraArgs(i), Integer?))
					ElseIf TypeOf extraArgs(i) Is Double? OrElse TypeOf extraArgs(i) Is Single? Then
						Dim num As Double? = DirectCast(extraArgs(i), Double?)
						customOpBuilder.addFloatingPointArguments(num)
					End If
				Next i
			End If

			Return customOpBuilder.build()

		End Function


		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

'JAVA TO VB CONVERTER NOTE: The variable baseOp was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim baseOp_Conflict As BaseOp = DirectCast(o, BaseOp)

			If If(x_Conflict IsNot Nothing, Not x_Conflict.Equals(baseOp_Conflict.x_Conflict), baseOp_Conflict.x_Conflict IsNot Nothing) Then
				Return False
			End If
			If If(y_Conflict IsNot Nothing, Not y_Conflict.Equals(baseOp_Conflict.y_Conflict), baseOp_Conflict.y_Conflict IsNot Nothing) Then
				Return False
			End If
			If If(z_Conflict IsNot Nothing, Not z_Conflict.Equals(baseOp_Conflict.z_Conflict), baseOp_Conflict.z_Conflict IsNot Nothing) Then
				Return False
			End If
			' Probably incorrect - comparing Object[] arrays with Arrays.equals
			If Not extraArgs.SequenceEqual(baseOp_Conflict.extraArgs) Then
				Return False
			End If
			Return If(extraArgz IsNot Nothing, extraArgz.Equals(baseOp_Conflict.extraArgz), baseOp_Conflict.extraArgz Is Nothing)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = MyBase.GetHashCode()
			result = 31 * result + (If(x_Conflict IsNot Nothing, x_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(y_Conflict IsNot Nothing, y_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(z_Conflict IsNot Nothing, z_Conflict.GetHashCode(), 0))
			result = 31 * result + Arrays.hashCode(extraArgs)
			result = 31 * result + (If(extraArgz IsNot Nothing, extraArgz.GetHashCode(), 0))
			Return result
		End Function

		Protected Friend Overridable Sub defineDimensions(ParamArray ByVal dimensions() As Integer)
			If dimensions IsNot Nothing AndAlso dimensions.Length > 0 Then
				If x_Conflict IsNot Nothing Then
					dimensions = Shape.normalizeAxis(x_Conflict.rank(), dimensions)
				End If
			End If

			If dimensions Is Nothing OrElse dimensions.Length = 0 Then
				dimensions = New Integer(){Integer.MaxValue}
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				Me.dimensionz = Shape.ndArrayDimFromInt(dimensions)
			End Using
		End Sub

		Public Overridable Function dimensions() As INDArray
			Return dimensionz
		End Function

		Public Overridable ReadOnly Property FinalResult As Number
			Get
				If Me.z_Conflict Is Nothing Then
					Throw New ND4JIllegalStateException("Op.Z is null. Op wasn't executed yet?")
				End If
    
				If z_Conflict.Empty Then
					Throw New ND4JIllegalStateException("Can't get number from empty array")
				End If
    
				If Not z_Conflict.Scalar Then
					Throw New ND4JIllegalStateException("Can't get final result scalar out of N-dim tensor")
				End If
    
				If z_Conflict.R Then
					Return New Double?(z_Conflict.getDouble(0))
				ElseIf z_Conflict.Z Then
					Return New Long?(z_Conflict.getInt(0))
				ElseIf z_Conflict.B Then
					Return New Integer?(z_Conflict.getInt(0))
				End If
    
				Throw New ND4JIllegalStateException("???")
			End Get
		End Property

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				'Always 1 for legacy/base ops
				Return 1
			End Get
		End Property

		Public Overrides Sub clearArrays() Implements Op.clearArrays
			x_Conflict = Nothing
			y_Conflict = Nothing
			z_Conflict = Nothing
		End Sub

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

	End Class

End Namespace