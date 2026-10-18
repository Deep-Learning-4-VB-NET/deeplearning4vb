Imports System
Imports System.Collections.Generic
Imports DifferentialFunction = DeepLearning4VB.org.nd4vb.autodiff.functions.DifferentialFunction
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If

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

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' The base class of the legacy ops: an op over the arrays x, y (optional) and z with extra arguments and
    ''' dimensions.
    ''' <para>
    ''' What an executioner reads: <see cref="x"/>, <see cref="y"/> and <see cref="z"/>; <see cref="extraArgs"/> (boxed
    ''' numbers, in the order the op class fills them) or <see cref="extraArgsDataBuff"/>; <see cref="dimensions"/>
    ''' (the normalized dimensions as an array, set by <see cref="defineDimensions"/>; <c>Integer.MaxValue</c> means
    ''' "all dimensions") and <see cref="getDimensions"/> (the dimensions as given); <see cref="opNum"/> and
    ''' <see cref="opName"/>.
    ''' </para>
    ''' <para>
    ''' Differences from Java: the fields <c>x</c>, <c>y</c> and <c>z</c> are named <see cref="xField"/>,
    ''' <see cref="yField"/> and <see cref="zField"/> (VB names are case-insensitive); the native-only member
    ''' <c>extraArgsBuff()</c> (a <c>java.nio</c> view) is not ported; <see cref="GetHashCode"/> uses only the
    ''' state <see cref="Equals"/> compares (x, y, z and the extra arguments), so equal ops have equal hash codes
    ''' (Java mixed in <c>DifferentialFunction.hashCode()</c>, which includes the random own name);
    ''' <see cref="toCustomOp"/> converts <c>Single</c> extra arguments to <c>Double</c> (Java's cast threw
    ''' <c>ClassCastException</c>). <c>getOpType(Op)</c> returns Nothing for ops of other kinds, as Java returned
    ''' null.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseOp
        Inherits DifferentialFunction
        Implements Op

        ''' <summary>The input array x (Java field <c>x</c>).</summary>
        Protected Friend xField As INDArray
        ''' <summary>The second input array y (Java field <c>y</c>).</summary>
        Protected Friend yField As INDArray
        ''' <summary>The output array z (Java field <c>z</c>).</summary>
        Protected Friend zField As INDArray

        Protected Friend xVertexId, yVertexId, zVertexId As String
        ' cached instance, for dataType checks
        Protected Friend extraArgz As DataBuffer

        Protected Friend dimensionz As INDArray

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inPlace As Boolean, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, inPlace, extraArgs)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal extraArgs() As Object)
			MyBase.New(sameDiff, extraArgs)
		End Sub
#End If

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
            Me.xField = x
            Me.yField = y
            Me.zField = z
        End Sub

        ''' <summary>
        ''' An op for one ndarray
        ''' </summary>
        ''' <param name="x"> the ndarray </param>
        Public Sub New(ByVal x As INDArray)
            Me.New(x, Nothing, x)
        End Sub

        ''' <summary>
        ''' The op type of an op as the grid and meta executioners see it: <c>CUSTOM</c> for custom ops,
        ''' <c>TRANSFORM_FLOAT</c> or <c>PAIRWISE</c> for transform ops (without or with y), the reduce op's own type
        ''' or <c>REDUCE3</c> (with y), <c>SCALAR</c>, <c>BROADCAST</c>, <c>INDEXREDUCE</c>, <c>META</c> or <c>GRID</c>;
        ''' Nothing for other ops.
        ''' </summary>
        Public Shared Function getOpType(ByVal op As Op) As Op.Type?
            Dim type As Op.Type? = Nothing

            If TypeOf op Is CustomOp Then
                Return Op.Type.CUSTOM
            ElseIf TypeOf op Is TransformOp Then
                If op.y() Is Nothing Then
                    type = Op.Type.TRANSFORM_FLOAT
                Else
                    type = Op.Type.PAIRWISE
                End If
            ElseIf TypeOf op Is ReduceOp Then
                If op.y() Is Nothing Then
                    type = DirectCast(op, ReduceOp).getOpType()
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
                type = Op.Type.META
            ElseIf TypeOf op Is GridOp Then
                type = Op.Type.GRID
            End If

            Return type
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
		End Sub
#End If

        ''' <summary>
        ''' The extra arguments as a constant buffer of the given type (integer types for integer and bool ops, double
        ''' for floating-point types), created on the first call and cached; Nothing when the op has no extra
        ''' arguments.
        ''' </summary>
        Public Overridable Function extraArgsDataBuff(ByVal dtype As DataType) As DataBuffer Implements Op.extraArgsDataBuff
            If extraArgz IsNot Nothing Then
                Return extraArgz
            End If

            If extraArgsField IsNot Nothing Then
                If ShapeT.isZ(dtype) OrElse ShapeT.isB(dtype) Then
                    Dim extraz(extraArgsField.Length - 1) As Long
                    For i As Integer = 0 To extraArgsField.Length - 1
                        If isNumber(extraArgsField(i)) Then
                            extraz(i) = longValueOf(extraArgsField(i))
                        End If
                    Next i
                    extraArgz = Nd4vbT.ConstantHandler.getConstantBuffer(extraz, dtype)
                    Return extraArgz
                ElseIf ShapeT.isR(dtype) Then
                    Dim extraz(extraArgsField.Length - 1) As Double
                    For i As Integer = 0 To extraArgsField.Length - 1
                        If Not isNumber(extraArgsField(i)) Then
                            Continue For
                        End If
                        extraz(i) = Convert.ToDouble(extraArgsField(i))
                    Next i
                    extraArgz = Nd4vbT.ConstantHandler.getConstantBuffer(extraz, dtype)
                    Return extraArgz
                End If
            End If

            Return Nothing
        End Function

        ''' <summary>True for the boxed numeric types that stand in for Java's <c>Number</c>.</summary>
        Friend Shared Function isNumber(ByVal value As Object) As Boolean
            Return TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Long OrElse
                TypeOf value Is Integer OrElse TypeOf value Is Short OrElse TypeOf value Is SByte OrElse
                TypeOf value Is Byte OrElse TypeOf value Is UShort OrElse TypeOf value Is UInteger OrElse
                TypeOf value Is ULong OrElse TypeOf value Is Decimal
        End Function

        ''' <summary>Java's <c>Number.longValue()</c>: floating values truncate toward zero and saturate, NaN gives 0.</summary>
        Friend Shared Function longValueOf(ByVal value As Object) As Long
            If TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal Then
                Dim d As Double = Convert.ToDouble(value)
                If Double.IsNaN(d) Then Return 0L
                If d >= 9.2233720368547758E+18 Then Return Long.MaxValue
                If d <= -9.2233720368547758E+18 Then Return Long.MinValue
                Return CLng(Math.Truncate(d))
            End If
            Return Convert.ToInt64(value)
        End Function

        ''' <summary>set x (the input ndarray)</summary>
        Public Overridable Sub setX(ByVal x As INDArray) Implements Op.setX
            Me.xField = x
        End Sub

        ''' <summary>set z (the solution ndarray)</summary>
        Public Overridable Sub setZ(ByVal z As INDArray) Implements Op.setZ
            Me.zField = z
        End Sub

        ''' <summary>set y (the pairwise ndarray)</summary>
        Public Overridable Sub setY(ByVal y As INDArray) Implements Op.setY
            Me.yField = y
        End Sub

        Public Overrides Sub setExtraArgs(ByVal extraArgs() As Object) Implements Op.setExtraArgs
            MyBase.setExtraArgs(extraArgs)
        End Sub

        Public Overridable Function extraArgs() As Object() Implements Op.extraArgs
            Return extraArgsField
        End Function

        Public Overridable Function x() As INDArray Implements Op.x
            Return xField
        End Function

        Public Overridable Function y() As INDArray Implements Op.y
            Return yField
        End Function

        Public Overridable Function z() As INDArray Implements Op.z
            Return zField
        End Function

        Public Overrides Function getInputArgument(ByVal index As Integer) As INDArray
            Preconditions.checkState(index >= 0 AndAlso index < 2, "Input argument index must be 0 or 1, got %s", index)
            Return If(index = 0, xField, yField)
        End Function

#If SAMEDIFF Then
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
#End If

        ''' <summary>The op number of a legacy op (Java <c>Op.opNum()</c>); op classes override it.</summary>
        Public Overrides Function opNum() As Integer Implements Op.opNum
            Return MyBase.opNum()
        End Function

        ''' <summary>The op name (Java <c>Op.opName()</c>), the key of the op in the managed op math registry; op classes override it.</summary>
        Public Overrides Function opName() As String Implements Op.opName
            Return MyBase.opName()
        End Function

        Public Overrides Function ToString() As String
            Return opName()
        End Function

        ''' <summary>
        ''' Converts this op to a <see cref="DynamicCustomOp"/> with the same name: x (and y) become the inputs, z the
        ''' output, Integer extra arguments become integer arguments and floating-point extra arguments become
        ''' floating-point arguments. The op name must be known to the executioner's custom op table.
        ''' </summary>
        Public Overridable Function toCustomOp() As CustomOp Implements Op.toCustomOp
            Dim customOpBuilder As DynamicCustomOp.DynamicCustomOpsBuilder = DynamicCustomOp.builder(opName())
            customOpBuilder.callInplace(x() Is z())

            If y() IsNot Nothing Then
                customOpBuilder.addInputs(x(), y())
            Else
                customOpBuilder.addInputs(x())
            End If

            customOpBuilder.addOutputs(z())
            If extraArgsField IsNot Nothing Then
                For i As Integer = 0 To extraArgsField.Length - 1
                    If TypeOf extraArgsField(i) Is Integer Then
                        customOpBuilder.addIntegerArguments(CLng(DirectCast(extraArgsField(i), Integer)))
                    ElseIf TypeOf extraArgsField(i) Is Double OrElse TypeOf extraArgsField(i) Is Single Then
                        Dim num As Double? = Convert.ToDouble(extraArgsField(i))
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
            If o Is Nothing OrElse CObj(Me).GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim other As BaseOp = DirectCast(o, BaseOp)

            If If(xField IsNot Nothing, Not xField.Equals(other.xField), other.xField IsNot Nothing) Then
                Return False
            End If
            If If(yField IsNot Nothing, Not yField.Equals(other.yField), other.yField IsNot Nothing) Then
                Return False
            End If
            If If(zField IsNot Nothing, Not zField.Equals(other.zField), other.zField IsNot Nothing) Then
                Return False
            End If
            ' Probably incorrect - comparing Object[] arrays with Arrays.equals
            If Not Arrays.equals(extraArgsField, other.extraArgsField) Then
                Return False
            End If
            Return If(extraArgz IsNot Nothing, extraArgz.Equals(other.extraArgz), other.extraArgz Is Nothing)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 31
            result = 31 * result + (If(xField IsNot Nothing, xField.GetHashCode(), 0))
            result = 31 * result + (If(yField IsNot Nothing, yField.GetHashCode(), 0))
            result = 31 * result + (If(zField IsNot Nothing, zField.GetHashCode(), 0))
            result = 31 * result + Arrays.hashCode(extraArgsField)
            result = 31 * result + (If(extraArgz IsNot Nothing, extraArgz.GetHashCode(), 0))
            Return result
        End Function

        ''' <summary>
        ''' Stores the dimensions as an array (<see cref="dimensions"/>): negative dimensions are normalized against
        ''' the rank of x, and no dimensions mean "all dimensions" (<c>Integer.MaxValue</c>).
        ''' </summary>
        Protected Friend Overridable Sub defineDimensions(ParamArray ByVal dimensions() As Integer)
            If dimensions IsNot Nothing AndAlso dimensions.Length > 0 Then
                If xField IsNot Nothing Then
                    dimensions = ShapeT.normalizeAxis(xField.rank(), dimensions)
                End If
            End If

            If dimensions Is Nothing OrElse dimensions.Length = 0 Then
                dimensions = New Integer() {Integer.MaxValue}
            End If

            Using ws As MemoryWorkspace = Nd4vbT.WorkspaceManager.scopeOutOfWorkspaces()
                Me.dimensionz = ShapeT.ndArrayDimFromInt(dimensions)
            End Using
        End Sub

        ''' <summary>The dimensions as an array, as stored by <see cref="defineDimensions"/> (Nothing if never set).</summary>
        Public Overridable Function dimensions() As INDArray
            Return dimensionz
        End Function

        ''' <summary>
        ''' The scalar result of an executed op: a boxed <c>Double</c> for floating-point z, a boxed <c>Long</c> for
        ''' integer z and a boxed <c>Integer</c> for bool z.
        ''' </summary>
        Public Overridable ReadOnly Property FinalResult As Object
            Get
                If Me.zField Is Nothing Then
                    Throw New ND4VBIllegalStateException("Op.Z is null. Op wasn't executed yet?")
                End If

                If zField.Empty Then
                    Throw New ND4VBIllegalStateException("Can't get number from empty array")
                End If

                If Not zField.Scalar Then
                    Throw New ND4VBIllegalStateException("Can't get final result scalar out of N-dim tensor")
                End If

                If zField.R Then
                    Return zField.getDouble(0L)
                ElseIf zField.Z Then
                    Return CLng(zField.getInt(0))
                ElseIf zField.B Then
                    Return zField.getInt(0)
                End If

                Throw New ND4VBIllegalStateException("???")
            End Get
        End Property

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                'Always 1 for legacy/base ops
                Return 1
            End Get
        End Property

        Public Overrides Sub clearArrays() Implements Op.clearArrays
            xField = Nothing
            yField = Nothing
            zField = Nothing
        End Sub

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

        ' ---------------------------------------------------------------- Lombok accessors

        Public Overridable Function getXVertexId() As String
            Return xVertexId
        End Function

        Public Overridable Sub setXVertexId(ByVal xVertexId As String)
            Me.xVertexId = xVertexId
        End Sub

        Public Overridable Function getYVertexId() As String
            Return yVertexId
        End Function

        Public Overridable Sub setYVertexId(ByVal yVertexId As String)
            Me.yVertexId = yVertexId
        End Sub

        Public Overridable Function getZVertexId() As String
            Return zVertexId
        End Function

        Public Overridable Sub setZVertexId(ByVal zVertexId As String)
            Me.zVertexId = zVertexId
        End Sub

    End Class

End Namespace
