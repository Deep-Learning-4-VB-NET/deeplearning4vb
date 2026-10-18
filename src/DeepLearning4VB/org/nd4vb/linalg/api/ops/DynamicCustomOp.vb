Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports DifferentialFunction = DeepLearning4VB.org.nd4vb.autodiff.functions.DifferentialFunction
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports OpDescriptorHolder = DeepLearning4VB.org.nd4vb.ir.OpDescriptorHolder
Imports OpNamespace = DeepLearning4VB.org.nd4vb.ir.OpNamespace
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
    ''' Basic implementation for CustomOp: an op with any number of input and output arrays and integer (iArgs),
    ''' floating-point (tArgs), boolean (bArgs), data type (dArgs) and string (sArgs) arguments, executed by
    ''' <see cref="opName"/>.
    ''' <para>
    ''' What an executioner reads: <see cref="opName"/>, <see cref="inputArguments"/>, <see cref="outputArguments"/>,
    ''' <see cref="iArgs"/>, <see cref="tArgs"/>, <see cref="bArgs"/>, <see cref="dArgs"/>, <see cref="sArgs"/> and
    ''' <see cref="InplaceCall"/>. <see cref="builder"/>, <see cref="opHash"/>, <see cref="opNum"/>,
    ''' <see cref="Descriptor"/> and <see cref="calculateOutputShape()"/> ask the executioner's custom op table.
    ''' </para>
    ''' <para>
    ''' Differences from Java: the fields <c>opName</c>, <c>inputArguments</c>, <c>outputArguments</c> and
    ''' <c>inplaceCall</c> are named <c>opNameField</c>, <see cref="inputArgumentsField"/>,
    ''' <see cref="outputArgumentsField"/> and <see cref="inplaceCallField"/> (VB names are case-insensitive);
    ''' Java's setter <c>setInputArguments(INDArray...)</c> keeps its name. <see cref="removeIArgument"/> removes the
    ''' first integer argument equal to the value (Java's <c>List&lt;Long&gt;.remove(Integer)</c> never matched);
    ''' the builder's <see cref="DynamicCustomOpsBuilder.build"/> copies the string arguments too (Java dropped
    ''' them). <see cref="opNum"/> truncates the hash to 32 bits like Java's <c>(int)</c> cast. The SameDiff members
    ''' are inside <c>#If SAMEDIFF</c>, the TensorFlow/ONNX members and <see cref="propertiesForFunction"/>'s use of
    ''' the op descriptors inside <c>#If MODEL_IMPORT</c>.
    ''' </para>
    ''' </summary>
    Public Class DynamicCustomOp
        Inherits DifferentialFunction
        Implements CustomOp

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DynamicCustomOp))

        Private opNameField As String
        ''' <summary>The input arrays (Java field <c>inputArguments</c>).</summary>
        Protected Friend inputArgumentsField As IList(Of INDArray) = New List(Of INDArray)()
        ''' <summary>The output arrays (Java field <c>outputArguments</c>).</summary>
        Protected Friend outputArgumentsField As IList(Of INDArray) = New List(Of INDArray)()
        Protected Friend tArguments As IList(Of Double) = New List(Of Double)()
        Protected Friend iArguments As IList(Of Long) = New List(Of Long)()
        Protected Friend bArguments As IList(Of Boolean) = New List(Of Boolean)()
        Protected Friend dArguments As IList(Of DataType) = New List(Of DataType)()
        Protected Friend sArguments As IList(Of String) = New List(Of String)()
        Protected Friend axis As IList(Of Integer) = New List(Of Integer)()

        ''' <summary>True when the op runs in place (Java field <c>inplaceCall</c>).</summary>
        Protected Friend inplaceCallField As Boolean

        Private hash As Long

#If SAMEDIFF Then
		Protected Friend outputVariables_Conflict() As SDVariable
#End If

        Private outputShapes As IList(Of LongShapeDescriptor)

        Public Sub New()
            iArguments = New List(Of Long)()
            tArguments = New List(Of Double)()
            bArguments = New List(Of Boolean)()
            dArguments = New List(Of DataType)()
            sArguments = New List(Of String)()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable)
			Me.New(sameDiff, wrapOrNull(arg))
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			Me.New(Nothing, sameDiff, args)
		End Sub

		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(sameDiff, args)
			Me.opName_Conflict = opName
			iArguments = New List(Of Long)()
			tArguments = New List(Of Double)()
			bArguments = New List(Of Boolean)()
			dArguments = New List(Of DataType)()
			sArguments = New List(Of String)()
		End Sub

		''' <summary>
		''' Initialize this for <seealso cref="SameDiff"/> execution
		''' Any extra int or float arguments for operations
		''' must be added to the respective TArguments
		''' or IArguments lists upon construction
		''' </summary>
		''' <param name="opName">   the operation opName </param>
		''' <param name="sameDiff"> the samediff instance to use </param>
		''' <param name="args">     the arguments to use </param>
		''' <param name="inPlace">  whether the operation is in place or not </param>
		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, inPlace, args)
			Me.opName_Conflict = opName
			iArguments = New List(Of Long)()
			tArguments = New List(Of Double)()
			bArguments = New List(Of Boolean)()
			dArguments = New List(Of DataType)()
			sArguments = New List(Of String)()
			Me.inplaceCall = inPlace
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			Me.New(Nothing, sameDiff, args, inPlace)
		End Sub
#End If

        Public Sub New(ByVal opName As String, ByVal input As INDArray, ByVal output As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments() As Integer)
            Me.New(opName, CType(If(input Is Nothing, Nothing, New INDArray() {input}), INDArray()), CType(If(output Is Nothing, Nothing, New INDArray() {output}), INDArray()), tArguments, iArguments)
        End Sub

        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments() As Integer)
            Me.New(opName, inputs, outputs, tArguments, ArrayUtil.toList(iArguments))
        End Sub

        ''' <summary>
        ''' Initialize this custom op with all of the
        ''' inputs, outputs, and respective
        ''' arguments for execution
        ''' </summary>
        ''' <param name="opName">     the opName of the op to execute </param>
        ''' <param name="inputs">     the inputs to the op </param>
        ''' <param name="outputs">    the outputs of the op </param>
        ''' <param name="tArguments"> the input float arguments </param>
        ''' <param name="iArguments"> the input int arguments </param>
        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments As IList(Of Integer))
            If inputs IsNot Nothing Then
                inputArgumentsField = New List(Of INDArray)(inputs)
            End If
            If outputs IsNot Nothing Then
                outputArgumentsField = New List(Of INDArray)(outputs)
            End If
            Me.opNameField = opName
            If tArguments Is Nothing Then
                Me.tArguments = New List(Of Double)()
            Else
                Me.tArguments = tArguments
            End If
            Me.iArguments = New List(Of Long)()
            Me.sArguments = New List(Of String)()

            If iArguments IsNot Nothing Then
                For Each a As Integer In iArguments
                    Me.iArguments.Add(a)
                Next a
            End If
            bArguments = New List(Of Boolean)()
            dArguments = New List(Of DataType)()
        End Sub

        ''' <summary>
        ''' Initialize this operation for execution (pre created ndarrays)
        ''' </summary>
        ''' <param name="inputs">  the inputs </param>
        ''' <param name="outputs"> the outputs of the op, may be null </param>
        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            Me.New(Nothing, inputs, outputs)
        End Sub

        ''' <summary>
        ''' Initialize this operation for execution (pre created ndarrays)
        ''' </summary>
        ''' <param name="opName">  the operation opName to use for invocation </param>
        ''' <param name="inputs">  the inputs </param>
        ''' <param name="outputs"> the outputs of the op, may be null </param>
        Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray)
            Me.New(opName, inputs, outputs, New List(Of Double)(), New List(Of Integer)())
        End Sub

        Protected Friend Sub New(ByVal opName As String)
            Me.opNameField = opName
            iArguments = New List(Of Long)()
            tArguments = New List(Of Double)()
            bArguments = New List(Of Boolean)()
            dArguments = New List(Of DataType)()
            sArguments = New List(Of String)()
        End Sub

        ''' <summary>
        ''' This method returns op opName as string
        ''' </summary>
        Public Overrides Function opName() As String Implements CustomOp.opName
            Return opNameField
        End Function

#If SAMEDIFF Then
		Public Overrides Function outputVariables() As SDVariable()
			Return outputVariables(If(getOwnName() IsNot Nothing, getOwnName(), opName()))
		End Function

		Public Overrides Function outputVariables(ByVal baseName As String) As SDVariable()
			If Me.outputVariables_Conflict Is Nothing Then
				Dim outputNames As val = sameDiff.getOutputsForOp(Me)
				'no need to dynamically create if already exists
				If outputNames IsNot Nothing Then
					outputVariables_Conflict = New SDVariable(outputNames.length - 1){}
					For i As Integer = 0 To outputVariables_Conflict.Length - 1
						outputVariables_Conflict(i) = sameDiff.getVariable(outputNames(i))
					Next i

					Return outputVariables_Conflict
				End If

				Dim newVars As val = sameDiff.generateOutputVariableForOp(Me, baseName, False) 'Also adds outgoing
				If InplaceCall Then
					If args().Length >= 1 Then
						Dim arr As val = args()(0).Arr
						If arr IsNot Nothing Then
							sameDiff.setArrayForVariable(newVars(0).name(), arr)
							addOutputArgument(arr)
						End If
					End If

					Return newVars
				End If

				outputVariables_Conflict = newVars
				If sameDiff.getOutputsForOp(Me) Is Nothing Then
					sameDiff.addOutgoingFor(outputVariables_Conflict, Me)
				End If
				Return newVars
			End If

			Return outputVariables_Conflict
		End Function
#End If

        ''' <summary>
        ''' This method returns LongHash of the opName(), from the executioner's custom op table (cached).
        ''' </summary>
        Public Overridable Function opHash() As Long Implements CustomOp.opHash
            If hash = 0 Then
                Dim map As IDictionary(Of String, CustomOpDescriptor) = Nd4vbT.Executioner.CustomOperations
                Dim desc As CustomOpDescriptor = map.GetValueOrNull(opName())
                If desc Is Nothing Then
                    Throw New ND4VBIllegalStateException("Op name " & opName() & " is missing!")
                End If

                hash = desc.getHash()
            End If

            Return hash
        End Function

        Public Overridable Function numDArguments() As Integer Implements CustomOp.numDArguments
            Return dArguments.Count
        End Function

        Public Overridable Function numSArguments() As Integer Implements CustomOp.numSArguments
            Return If(sArguments Is Nothing, 0, sArguments.Count)
        End Function

        Public Overridable Function outputArguments() As IList(Of INDArray) Implements CustomOp.outputArguments
            Return outputArgumentsField
        End Function

        Public Overridable Function inputArguments() As IList(Of INDArray) Implements CustomOp.inputArguments
            Return inputArgumentsField
        End Function

        Public Overridable Function iArgs() As Long() Implements CustomOp.iArgs
            Return iArguments.ToArray()
        End Function

        Public Overridable Function tArgs() As Double() Implements CustomOp.tArgs
            Return tArguments.ToArray()
        End Function

        Public Overridable Function dArgs() As DataType() Implements CustomOp.dArgs
            Return dArguments.ToArray()
        End Function

        Public Overridable Function sArgs() As String() Implements CustomOp.sArgs
            Return sArguments.ToArray()
        End Function

        Public Overridable Sub addIArgument(ParamArray ByVal arg() As Integer) Implements CustomOp.addIArgument
            For Each a As Long In arg
                iArguments.Add(a)
            Next a
        End Sub

        Public Overridable Sub addIArgument(ParamArray ByVal arg() As Long) Implements CustomOp.addIArgument
            For Each a As Long In arg
                iArguments.Add(a)
            Next a
        End Sub

        ''' <summary>Removes the first integer argument equal to <paramref name="arg"/> (see the class remarks).</summary>
        Public Overridable Sub removeIArgument(ByVal arg As Integer?) Implements CustomOp.removeIArgument
            If arg.HasValue Then
                iArguments.Remove(CLng(arg.Value))
            End If
        End Sub

        Public Overridable Sub addSArgument(ParamArray ByVal args() As String) Implements CustomOp.addSArgument
            For Each s As String In args
                sArguments.Add(s)
            Next s
        End Sub

        Public Overridable Sub removeSArgument(ByVal argument As String) Implements CustomOp.removeSArgument
            sArguments.Remove(argument)
        End Sub

        Public Overridable Function getSArgument(ByVal index As Integer) As String Implements CustomOp.getSArgument
            Return sArguments(index)
        End Function

        Public Overridable Function getIArgument(ByVal index As Integer) As Long? Implements CustomOp.getIArgument
            Return iArguments(index)
        End Function

        Public Overridable Function numIArguments() As Integer Implements CustomOp.numIArguments
            Return If(iArguments Is Nothing, 0, iArguments.Count)
        End Function

        Public Overridable Function numBArguments() As Integer Implements CustomOp.numBArguments
            Return If(bArguments Is Nothing, 0, bArguments.Count)
        End Function

        Public Overridable Sub addTArgument(ParamArray ByVal arg() As Double) Implements CustomOp.addTArgument
            If arg IsNot Nothing Then
                For Each d As Double In arg
                    tArguments.Add(d)
                Next d
            End If
        End Sub

        Public Overridable Sub addDArgument(ParamArray ByVal arg() As DataType) Implements CustomOp.addDArgument
            If dArguments Is Nothing Then
                dArguments = New List(Of DataType)()
            End If

            If arg IsNot Nothing Then
                For Each d As DataType In arg
                    dArguments.Add(d)
                Next d
            End If
        End Sub

        Public Overridable Sub removeTArgument(ByVal arg As Double?) Implements CustomOp.removeTArgument
            If arg.HasValue Then
                tArguments.Remove(arg.Value)
            End If
        End Sub

        Public Overridable Function getTArgument(ByVal index As Integer) As Double? Implements CustomOp.getTArgument
            Return tArguments(index)
        End Function

        Public Overridable Function numTArguments() As Integer Implements CustomOp.numTArguments
            Return If(tArguments Is Nothing, 0, tArguments.Count)
        End Function

        Public Overridable Sub addInputArgument(ParamArray ByVal arg() As INDArray) Implements CustomOp.addInputArgument
            For i As Integer = 0 To arg.Length - 1
                If arg(i) Is Nothing Then
                    Throw New ND4VBIllegalStateException("Input " & i & " was null!")
                End If
            Next i

            For Each a As INDArray In arg
                inputArgumentsField.Add(a)
            Next a
#If SAMEDIFF Then
			Dim args As val = If(sameDiff IsNot Nothing, Me.args(), Nothing)
			Dim arrsSoFar As val = inputArguments()
			'validate arrays passed in, keep in mind that
			'this is a cumulative algorithm so we should always
			'refresh the current list
			If args IsNot Nothing Then
				For i As Integer = 0 To args.length - 1

					' it's possible to get into situation where number of args > number of arrays AT THIS MOMENT
					If i >= arrsSoFar.size() Then
						Continue For
					End If

					If Not args(i).getShape().SequenceEqual(arrsSoFar.get(i).shape()) Then
						Throw New ND4JIllegalStateException("Illegal array passed in as argument [" & i & "]. Expected shape " & java.util.Arrays.toString(args(i).getShape()) & " and received array with shape " & java.util.Arrays.toString(arg(i).shape()))
					End If
				Next i
			End If
#End If
        End Sub

        Public Overridable Sub removeInputArgument(ByVal arg As INDArray) Implements CustomOp.removeInputArgument
            inputArgumentsField.Remove(arg)
        End Sub

        Public Overrides Function getInputArgument(ByVal index As Integer) As INDArray Implements CustomOp.getInputArgument
            If inputArgumentsField Is Nothing OrElse index >= inputArgumentsField.Count Then
                Return Nothing
            End If
            Return inputArgumentsField(index)
        End Function

        Public Overridable Sub setInputArgument(ByVal index As Integer, ByVal input As INDArray)
            If index >= inputArgumentsField.Count Then
                Dim oldArgs As IList(Of INDArray) = inputArgumentsField
                inputArgumentsField = New List(Of INDArray)(index + 1)
                For Each a As INDArray In oldArgs
                    inputArgumentsField.Add(a)
                Next a
                Do While inputArgumentsField.Count <= index
                    inputArgumentsField.Add(Nothing)
                Loop
            End If
            inputArgumentsField(index) = input
        End Sub

        ''' <summary>Replaces the input arrays (Java <c>setInputArguments(INDArray...)</c>).</summary>
        Public Overridable Sub setInputArguments(ParamArray ByVal inputs() As INDArray)
            inputArgumentsField.Clear()
            If inputs IsNot Nothing AndAlso inputs.Length > 0 Then
                For Each a As INDArray In inputs
                    inputArgumentsField.Add(a)
                Next a
            End If
        End Sub

        Public Overridable Sub setOutputArgument(ByVal index As Integer, ByVal output As INDArray)
            Do While index >= outputArgumentsField.Count
                'Resize list, in case we want to specify arrays not in order they are defined
                'For example, index 1 on empty list, then index 0
                outputArgumentsField.Add(Nothing)
            Loop
            outputArgumentsField(index) = output
        End Sub

        Public Overridable Function numInputArguments() As Integer Implements CustomOp.numInputArguments
            Return inputArgumentsField.Count
        End Function

        Public Overridable Sub addOutputArgument(ParamArray ByVal arg() As INDArray) Implements CustomOp.addOutputArgument
            For i As Integer = 0 To arg.Length - 1
                If arg(i) Is Nothing Then
                    Throw New ND4VBIllegalStateException("Output " & i & " was null!")
                End If
            Next i
            For Each a As INDArray In arg
                outputArgumentsField.Add(a)
            Next a
        End Sub

        Public Overridable Sub removeOutputArgument(ByVal arg As INDArray) Implements CustomOp.removeOutputArgument
            outputArgumentsField.Remove(arg)
        End Sub

        Public Overridable Function getOutputArgument(ByVal index As Integer) As INDArray Implements CustomOp.getOutputArgument
            If outputArgumentsField Is Nothing OrElse index >= outputArgumentsField.Count Then
                Return Nothing
            End If
            Return outputArgumentsField(index)
        End Function

        Public Overridable Function numOutputArguments() As Integer Implements CustomOp.numOutputArguments
            Return outputArgumentsField.Count
        End Function

        ''' <summary>The op hash truncated to 32 bits (Java <c>(int) opHash()</c>).</summary>
        Public Overrides Function opNum() As Integer
            Return Arrays.WrapToInt32(opHash())
        End Function

        ''' <summary>
        ''' This method takes custom opname, and return Op DynamicCustomOpsBuilder instance. The name (or its lower-case
        ''' form) must be in the executioner's custom op table.
        ''' </summary>
        ''' <param name="opName"> the op name </param>
        Public Shared Function builder(ByVal opName As String) As DynamicCustomOpsBuilder
            Dim map As IDictionary(Of String, CustomOpDescriptor) = Nd4vbT.Executioner.CustomOperations
            Dim lcName As String = If(map.ContainsKey(opName), opName, opName.ToLowerInvariant())
            Dim desc As CustomOpDescriptor = map.GetValueOrNull(lcName)

            If desc Is Nothing Then
                Throw New ND4VBIllegalStateException("Unknown operations requested: [" & opName & "]")
            End If

            Return New DynamicCustomOpsBuilder(lcName, desc.getHash(), desc.getNumInputs(), desc.getNumOutputs(), desc.isAllowsInplace(), desc.getNumTArgs(), desc.getNumIArgs())
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor) Implements CustomOp.calculateOutputShape
            Return calculateOutputShape(Nothing)
        End Function

        Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor) Implements CustomOp.calculateOutputShape
            Dim descriptor As CustomOpDescriptor = Me.Descriptor
            If outputShapes IsNot Nothing AndAlso outputShapes.Count > 0 Then
                Return outputShapes
            End If

            If descriptor Is Nothing Then
                Throw New InvalidOperationException("Could not find descriptor for op: " & opName() & (If(GetType(DynamicCustomOp) Is CObj(Me).GetType(), "", " - class: " & CObj(Me).GetType().FullName)))
            End If

            'not fully initialized: missing integer args
            Dim nI As Integer = If(oc IsNot Nothing, oc.numIArguments(), numIArguments())
            If descriptor.getNumIArgs() >= 0 AndAlso nI < descriptor.getNumIArgs() Then
                If log.isTraceEnabled() Then
                    log.trace("Could not calculate output shape for op {}: not fully initialized ({} IArgs specified, " & "{} required)", CObj(Me).GetType().FullName, nI, descriptor.getNumIArgs())
                End If
                Return New List(Of LongShapeDescriptor)()
            End If

            'not fully initialized: missing floating point args
            Dim nT As Integer = If(oc IsNot Nothing, oc.numTArguments(), numTArguments())
            If descriptor.getNumTArgs() >= 0 AndAlso nT < descriptor.getNumTArgs() Then
                If log.isTraceEnabled() Then
                    log.trace("Could not calculate output shape for op {}: not fully initialized ({} TArgs specified, " & "{} required)", CObj(Me).GetType().FullName, nT, descriptor.getNumTArgs())
                End If
                Return New List(Of LongShapeDescriptor)()
            End If

            'not fully initialized: missing INDArray input args
            Dim nIn As Integer = If(oc IsNot Nothing, oc.numInputArguments(), numInputArguments())
            If descriptor.getNumInputs() >= 0 AndAlso nIn < descriptor.getNumInputs() Then
                If log.isTraceEnabled() Then
                    log.trace("Could not calculate output shape for op {}: not fully initialized ({} input (INDArray) args specified, " & "{} required)", CObj(Me).GetType().FullName, nIn, descriptor.getNumInputs())
                End If
                Return New List(Of LongShapeDescriptor)()
            End If

            Dim ret As IList(Of LongShapeDescriptor)
            If oc Is Nothing Then
                ret = Nd4vbT.Executioner.calculateOutputShape(Me)
            Else
                ret = Nd4vbT.Executioner.calculateOutputShape(Me, oc)
            End If
            Return ret
        End Function

        ''' <summary>The descriptor of the op in the executioner's custom op table, or Nothing.</summary>
        Public Overridable ReadOnly Property Descriptor As CustomOpDescriptor Implements CustomOp.Descriptor
            Get
                Dim map As IDictionary(Of String, CustomOpDescriptor) = Nd4vbT.Executioner.CustomOperations
                Return map.GetValueOrNull(opName())
            End Get
        End Property

        Public Overridable Sub assertValidForExecution() Implements CustomOp.assertValidForExecution
            Dim descriptor As CustomOpDescriptor = Me.Descriptor
            If descriptor Is Nothing Then
                Throw New NoOpNameFoundException("No descriptor found for op name " & opName())
            End If

            If descriptor.getNumInputs() > 0 AndAlso numInputArguments() < descriptor.getNumInputs() Then
#If SAMEDIFF Then
				If sameDiff Is Nothing Then
					Throw New ND4JIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of inputs is invalid for execution. " & numInputArguments() & " were provided but " & descriptor.getNumInputs() & " are required for execution")
				Else
					Dim inputNames() As String = sameDiff.getInputsForOp(Me)
					Dim arrayShapes(inputNames.Length - 1) As String
					For i As Integer = 0 To inputNames.Length - 1
						Dim arr As INDArray = sameDiff.getVariable(inputNames(i)).Arr
						arrayShapes(i) = (If(arr Is Nothing, "<no array present>", java.util.Arrays.toString(arr.shape())))
					Next i
					Throw New ND4JIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of inputs is invalid for execution. " & numInputArguments() & " were provided but " & descriptor.getNumInputs() & " are required for execution. Input variable names: " & java.util.Arrays.toString(inputNames) & ". Input variable array shapes: " & java.util.Arrays.toString(arrayShapes))
				End If
#Else
                Throw New ND4VBIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of inputs is invalid for execution. " & numInputArguments() & " were provided but " & descriptor.getNumInputs() & " are required for execution")
#End If
            End If

            If descriptor.getNumOutputs() > 0 AndAlso numOutputArguments() < descriptor.getNumOutputs() Then
                Throw New ND4VBIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of outputs is invalid for execution. Specified [" & numOutputArguments() & "] but should be [" & descriptor.getNumOutputs() & "]")
            End If

            '< 0 means dynamic size
            If descriptor.getNumIArgs() >= 0 AndAlso numIArguments() < descriptor.getNumIArgs() Then
                Throw New ND4VBIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of integer arguments is invalid for execution. Specified [" & numIArguments() & "] but should be [" & descriptor.getNumIArgs() & "]")
            End If

            If descriptor.getNumTArgs() >= 0 AndAlso numTArguments() < descriptor.getNumTArgs() Then
                Throw New ND4VBIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of inputs is invalid for execution. Specified [" & numTArguments() & "] but should be [" & descriptor.getNumTArgs() & "]")
            End If
        End Sub

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Please extend DynamicCustomOp.doDiff to support SameDiff backprop " & "operations. Op: " & Me.GetType().FullName)
		End Function
#End If

        Public Overrides Function ToString() As String
            Return opName()
        End Function

        Public Overridable Function bArgs() As Boolean() Implements CustomOp.bArgs
            Dim result(If(bArguments Is Nothing, 0, bArguments.Count) - 1) As Boolean
            For e As Integer = 0 To result.Length - 1
                result(e) = bArguments(e)
            Next e
            Return result
        End Function

        Public Overridable Sub addBArgument(ParamArray ByVal arg() As Boolean) Implements CustomOp.addBArgument
            If arg IsNot Nothing Then
                For Each b As Boolean In arg
                    bArguments.Add(b)
                Next b
            End If
        End Sub

        Public Overridable Function getBArgument(ByVal index As Integer) As Boolean? Implements CustomOp.getBArgument
            Return bArguments(index)
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub
#End If

        Public Overrides Sub clearArrays() Implements CustomOp.clearArrays
            inputArgumentsField.Clear()
            outputArgumentsField.Clear()
        End Sub

#If SAMEDIFF Then
		Protected Friend Shared Function wrapOrNull(ByVal [in] As SDVariable) As SDVariable()
			Return If([in] Is Nothing, Nothing, New SDVariable()){[in]}
		End Function
#End If

        Protected Friend Shared Function wrapOrNull(ByVal [in] As INDArray) As INDArray()
            Return If([in] Is Nothing, Nothing, New INDArray() {[in]})
        End Function

        Protected Friend Shared Function wrapFilterNull(Of T)(ParamArray ByVal [in]() As T) As T()
            Dim count As Integer = 0
            For i As Integer = 0 To [in].Length - 1
                If [in](i) IsNot Nothing Then
                    count += 1
                End If
            Next i
            Dim [out]() As T = CType(Array.CreateInstance([in].GetType().GetElementType(), count), T())
            Dim j As Integer = 0
            For i As Integer = 0 To [in].Length - 1
                If [in](i) IsNot Nothing Then
                    [out](j) = [in](i)
                    j += 1
                End If
            Next i
            Return [out]
        End Function

        ''' <summary>True when the op runs in place (Java <c>isInplaceCall()</c>).</summary>
        Public Overridable ReadOnly Property InplaceCall As Boolean Implements CustomOp.InplaceCall
            Get
                Return inplaceCallField
            End Get
        End Property

        Public Overridable Sub setInplaceCall(ByVal inplaceCall As Boolean)
            Me.inplaceCallField = inplaceCall
        End Sub

        ''' <summary>The cached op hash (Lombok's <c>getHash()</c>); 0 until <see cref="opHash"/> is called or the builder sets it.</summary>
        Public Overridable Function getHash() As Long
            Return hash
        End Function

        ''' <summary>
        ''' Builds a <see cref="DynamicCustomOp"/> (Java <c>DynamicCustomOp.DynamicCustomOpsBuilder</c>). The numbers of
        ''' inputs, outputs and arguments come from the op's descriptor; a negative number means "any".
        ''' </summary>
        Public Class DynamicCustomOpsBuilder
            Protected Friend opName As String
            Protected Friend numInputs As Integer
            Protected Friend numOutputsField As Integer
            Protected Friend numTArguments As Integer
            Protected Friend numIArguments As Integer
            Protected Friend numBArguments As Integer
            Protected Friend numSArguments As Integer
            Protected Friend inplaceCall As Boolean
            Protected Friend inplaceAllowed As Boolean
            Protected Friend opHash As Long
            Protected Friend outputShapes As IList(Of LongShapeDescriptor) = New List(Of LongShapeDescriptor)()

            Friend inputArguments As IList(Of INDArray) = New List(Of INDArray)()
            Friend outputArguments As IList(Of INDArray) = New List(Of INDArray)()
            Friend tArguments As IList(Of Double) = New List(Of Double)()
            Friend iArguments As IList(Of Long) = New List(Of Long)()
            Friend dArguments As IList(Of DataType) = New List(Of DataType)()
            Friend bArguments As IList(Of Boolean) = New List(Of Boolean)()
            Friend sArguments As IList(Of String) = New List(Of String)()

            Protected Friend Sub New(ByVal opName As String, ByVal hash As Long, ByVal numInputs As Integer, ByVal numOutputs As Integer, ByVal inplaceAllowed As Boolean, ByVal numTArguments As Integer, ByVal numIArguments As Integer)
                Me.New(opName, hash, numInputs, numOutputs, inplaceAllowed, numTArguments, numIArguments, 0)
            End Sub

            Protected Friend Sub New(ByVal opName As String, ByVal hash As Long, ByVal numInputs As Integer, ByVal numOutputs As Integer, ByVal inplaceAllowed As Boolean, ByVal numTArguments As Integer, ByVal numIArguments As Integer, ByVal numSArguments As Integer)
                Me.opHash = hash
                Me.opName = opName
                Me.numInputs = numInputs
                Me.numOutputsField = numOutputs
                Me.numIArguments = numIArguments
                Me.numTArguments = numTArguments
                Me.numSArguments = numSArguments
                Me.inplaceAllowed = inplaceAllowed
            End Sub

            ''' <summary>
            ''' This method
            ''' takes arbitrary number of input INDArrays in, as Op input
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate lengths/shapes.
            ''' </summary>
            ''' <param name="inputs"> the inputs </param>
            Public Overridable Function addInputs(ParamArray ByVal inputs() As INDArray) As DynamicCustomOpsBuilder
                ' if we have positive value as numInputs - we should ensure equal amount of arguments
                If numInputs >= 0 Then
                    If inputs Is Nothing Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numInputs & " arguments. Null was passed instead.")
                    End If

                    If numInputs > inputs.Length Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numInputs & " arguments, but " & inputs.Length & " was passed to constructor")
                    End If
                End If

                For Each [in] As INDArray In inputs
                    inputArguments.Add([in])
                Next [in]

                Return Me
            End Function

            ''' <summary>
            ''' This method takes arbitrary number of
            ''' output INDArrays in, to store operation result
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate lengths/shapes.
            ''' </summary>
            ''' <param name="outputs"> the outputs </param>
            Public Overridable Function addOutputs(ParamArray ByVal outputs() As INDArray) As DynamicCustomOpsBuilder
                If numOutputsField >= 0 Then
                    If outputs Is Nothing Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numOutputsField & " arguments. Null was passed instead.")
                    End If

                    If numOutputsField > outputs.Length Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numOutputsField & " arguments, but " & outputs.Length & " was passed to constructor")
                    End If
                End If

                For Each [in] As INDArray In outputs
                    outputArguments.Add([in])
                Next [in]

                Return Me
            End Function

            ''' <summary>
            ''' Whether an op call is in place or not.
            ''' </summary>
            ''' <param name="reallyCall"> True for an in-place call (the op must allow it) </param>
            Public Overridable Function callInplace(ByVal reallyCall As Boolean) As DynamicCustomOpsBuilder
                If reallyCall AndAlso Not inplaceAllowed Then
                    Throw New ND4VBIllegalStateException("Requested op can't be called inplace")
                End If

                Me.inplaceCall = reallyCall

                Return Me
            End Function

            ''' <summary>
            ''' This method takes arbitrary number of Integer arguments for op,
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate values.
            ''' </summary>
            ''' <param name="iargs"> the integer arguments </param>
            Public Overridable Function addIntegerArguments(ByVal iargs As IList(Of Integer)) As DynamicCustomOpsBuilder
                If numIArguments >= 0 Then
                    If iargs Is Nothing Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects " & numIArguments & " integer arguments. Null was passed instead.")
                    End If

                    If numIArguments > iargs.Count Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numIArguments & " integer arguments, but " & iargs.Count & " was passed to constructor")
                    End If
                End If

                For Each [in] As Integer In iargs
                    iArguments.Add([in])
                Next [in]

                Return Me
            End Function

            ''' <summary>
            ''' This method takes arbitrary number of String arguments for op,
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate values.
            ''' </summary>
            ''' <param name="sArgs"> the string arguments </param>
            Public Overridable Function addStringArguments(ByVal sArgs As IList(Of String)) As DynamicCustomOpsBuilder
                If numSArguments >= 0 Then
                    If sArgs Is Nothing Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects " & numSArguments & " string arguments. Null was passed instead.")
                    End If

                    If numSArguments > sArgs.Count Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numSArguments & " string arguments, but " & sArgs.Count & " was passed to constructor")
                    End If
                End If

                For Each [in] As String In sArgs
                    sArguments.Add([in])
                Next [in]

                Return Me
            End Function

            ''' <summary>
            ''' This method takes one String argument for op,
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate values.
            ''' </summary>
            ''' <param name="arg"> the string argument </param>
            Public Overridable Function addStringArguments(ByVal arg As String) As DynamicCustomOpsBuilder
                If numSArguments <> 1 AndAlso numSArguments > 0 Then
                    Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects " & numSArguments & " string arguments. One arg was passed instead.")
                End If

                sArguments.Add(arg)

                Return Me
            End Function

            ''' <summary>
            ''' This method takes arbitrary number of String arguments for op,
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate values.
            ''' </summary>
            ''' <param name="sArgs"> the string arguments </param>
            Public Overridable Function addStringArguments(ParamArray ByVal sArgs() As String) As DynamicCustomOpsBuilder
                If numSArguments >= 0 Then
                    If sArgs Is Nothing Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numSArguments & " integer arguments. Null was passed instead.")
                    End If

                    If numSArguments > sArgs.Length Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numSArguments & " integer arguments, but " & sArgs.Length & " was passed to constructor")
                    End If
                End If

                For Each [in] As String In sArgs
                    sArguments.Add([in])
                Next [in]

                Return Me
            End Function

            ''' <summary>
            ''' This method takes one Integer argument for op,
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate values.
            ''' </summary>
            ''' <param name="arg"> the integer argument </param>
            Public Overridable Function addIntegerArguments(ByVal arg As Long) As DynamicCustomOpsBuilder
                If numIArguments <> 1 AndAlso numIArguments > 0 Then
                    Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects " & numIArguments & " integer arguments. One arg was passed instead.")
                End If

                iArguments.Add(arg)

                Return Me
            End Function

            ''' <summary>
            ''' This method takes arbitrary number of Integer arguments for op,
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate values.
            ''' </summary>
            ''' <param name="iargs"> the integer arguments </param>
            Public Overridable Function addIntegerArguments(ParamArray ByVal iargs() As Integer) As DynamicCustomOpsBuilder
                If numIArguments >= 0 Then
                    If iargs Is Nothing Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numIArguments & " integer arguments. Null was passed instead.")
                    End If

                    If numIArguments > iargs.Length Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numIArguments & " integer arguments, but " & iargs.Length & " was passed to constructor")
                    End If
                End If

                For Each [in] As Integer In iargs
                    iArguments.Add(CLng([in]))
                Next [in]

                Return Me
            End Function

            ''' <summary>
            ''' This method takes arbitrary number of Boolean arguments for op,
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate values.
            ''' </summary>
            ''' <param name="bargs"> the boolean arguments </param>
            Public Overridable Function addBooleanArguments(ParamArray ByVal bargs() As Boolean) As DynamicCustomOpsBuilder
                For Each [in] As Boolean In bargs
                    bArguments.Add([in])
                Next [in]
                Return Me
            End Function

            ''' <summary>
            ''' This method takes arbitrary number of Double arguments for op,
            ''' Note that this ACCUMULATES arguments. You are able to call this method
            ''' multiple times and it will add arguments to a list.
            ''' PLEASE NOTE: this method does NOT validate values.
            ''' </summary>
            Public Overridable Function addFloatingPointArguments(ParamArray ByVal targs() As Double?) As DynamicCustomOpsBuilder
                If numTArguments >= 0 Then
                    If targs Is Nothing Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numTArguments & " integer arguments. Null was passed instead.")
                    End If

                    If numTArguments > targs.Length Then
                        Throw New ND4VBIllegalStateException("CustomOp [" & opName & "] expects at least " & numTArguments & " integer arguments, but " & targs.Length & " was passed to constructor")
                    End If
                End If

                For Each [in] As Double? In targs
                    tArguments.Add([in].Value)
                Next [in]

                Return Me
            End Function

            ''' <summary>
            ''' Adds an output shape
            ''' </summary>
            ''' <param name="shape"> the shape descriptor </param>
            Public Overridable Function addOutputShape(ByVal shape As LongShapeDescriptor) As DynamicCustomOpsBuilder
                Me.outputShapes.Add(shape)
                Return Me
            End Function

            Public Overridable Function build() As DynamicCustomOp
                ' Eventually we probably will lift this restriction
                'if (!inplaceCall && outputArguments.size() == 0)
                '    throw new ND4JIllegalStateException("If operation is not-inplace, it must have outputs defined");

                Dim result As New DynamicCustomOp(opName)
                result.inputArgumentsField = inputArguments
                result.outputArgumentsField = outputArguments
                result.iArguments = iArguments
                result.tArguments = tArguments
                result.bArguments = bArguments
                result.dArguments = dArguments
                result.sArguments = sArguments
                result.inplaceCallField = inplaceCall
                result.hash = opHash
                result.outputShapes = outputShapes

                Return result
            End Function

            Public Overridable ReadOnly Property NumOutputs As Integer
                Get
                    Return -1
                End Get
            End Property
        End Class

        Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
            Return MyBase.mappingsForFunction()
        End Function

        Public Overrides Sub setPropertiesForFunction(ByVal properties As IDictionary(Of String, Object))
            MyBase.setPropertiesForFunction(properties)
        End Sub

        Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
            Return MyBase.getValue([property])
        End Function

        Public Overrides Sub setValueFor(ByVal target As System.Reflection.FieldInfo, ByVal value As Object)
        End Sub

        ''' <summary>
        ''' The arguments of the op by name, from the op's descriptor in the op descriptor table of model import, so
        ''' outside <c>#If MODEL_IMPORT</c> this throws <see cref="NotSupportedException"/>.
        ''' </summary>
        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
#If MODEL_IMPORT Then
			Dim opDescriptor As OpNamespace.OpDescriptor = OpDescriptorHolder.descriptorForOpName(opName())
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			For Each argDescriptor As OpNamespace.ArgDescriptor In opDescriptor.getArgDescriptorList()
				Select Case argDescriptor.ArgType
					Case [STRING]
						If argDescriptor.ArgIndex < numSArguments() Then
							ret(argDescriptor.Name) = getSArgument(argDescriptor.ArgIndex)
						End If
					Case BOOL
						If argDescriptor.ArgIndex < numBArguments() Then
							ret(argDescriptor.Name) = getBArgument(argDescriptor.ArgIndex)
						End If
					Case FLOAT, [DOUBLE]
						If argDescriptor.ArgIndex < numTArguments() Then
							ret(argDescriptor.Name) = getTArgument(argDescriptor.ArgIndex)
						End If
					Case INT32, INT64
						If argDescriptor.ArgIndex < numIArguments() Then
							ret(argDescriptor.Name) = getIArgument(argDescriptor.ArgIndex)
						End If
					Case DATA_TYPE
						If argDescriptor.ArgIndex < numDArguments() Then
							ret(argDescriptor.Name) = dArguments(argDescriptor.ArgIndex)
						End If
				End Select
			Next argDescriptor
			Return ret
#Else
            Throw New NotSupportedException("propertiesForFunction needs the op descriptors of model import (phase 3, samediff-import)")
#End If
        End Function

    End Class

End Namespace
