Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports AttributeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
Imports OpContext = DeepLearning4VB.org.nd4vb.linalg.api.ops.OpContext
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If
#If FLATBUFFERS Then
Imports FlatBuffersMapper = DeepLearning4VB.org.nd4vb.autodiff.samediff.serde.FlatBuffersMapper
#End If
#If MODEL_IMPORT Then
Imports Onnx = onnx.Onnx
Imports DifferentialFunctionClassHolder = DeepLearning4VB.org.nd4vb.imports.converters.DifferentialFunctionClassHolder
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

Namespace org.nd4vb.autodiff.functions

    ''' <summary>
    ''' The base class of every op: the legacy ops (<c>BaseOp</c>) and the custom ops (<c>DynamicCustomOp</c>).
    ''' It holds the state that both share (in-place flag, scalar value, dimensions, extra arguments, own name), the
    ''' property mapping used by model import, and the SameDiff graph members.
    ''' <para>
    ''' Fences: the SameDiff graph members (<c>sameDiff</c>, the constructors taking a <c>SameDiff</c>,
    ''' <c>args()</c>, <c>arg()</c>, <c>larg()</c>, <c>rarg()</c>, <c>outputVariables</c>, <c>doDiff</c>, <c>diff</c>,
    ''' ...) are inside <c>#If SAMEDIFF</c>; the TensorFlow/ONNX members (<c>initFromTensorFlow</c>,
    ''' <c>initFromOnnx</c>, the constructors taking a <c>NodeDef</c> or <c>NodeProto</c>) and the uses of the import
    ''' machinery (<c>DifferentialFunctionClassHolder</c> in <see cref="propertiesForFunction"/> and
    ''' <see cref="setPropertiesForFunction"/>) are inside <c>#If MODEL_IMPORT</c>; <see cref="dup"/>, which clones
    ''' through FlatBuffers, is inside <c>#If FLATBUFFERS</c>. Outside the fences those members that callers need
    ''' throw <see cref="NotSupportedException"/>.
    ''' </para>
    ''' <para>
    ''' Differences from Java: the fields <c>dimensions</c> and <c>extraArgs</c> are named
    ''' <see cref="dimensionsField"/> and <see cref="extraArgsField"/> (VB names are case-insensitive and
    ''' <c>BaseOp</c> has the methods <c>dimensions()</c> and <c>extraArgs()</c>); Lombok's accessors are written out
    ''' with their Java names (<see cref="getDimensions"/>, <see cref="setExtraArgs"/>, <see cref="isInPlace"/>, ...),
    ''' but Lombok's <c>toString()</c> is not (every op class overrides <see cref="ToString"/>). Java's
    ''' <c>java.lang.reflect.Field</c> is <see cref="FieldInfo"/>. Outside SameDiff the own name is a random GUID, as
    ''' Java's random UUID.
    ''' </para>
    ''' </summary>
    Public MustInherit Class DifferentialFunction

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DifferentialFunction))

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @JsonIgnore protected org.nd4j.autodiff.samediff.SameDiff sameDiff;
		Protected Friend sameDiff As SameDiff
#End If

        Protected Friend inPlace As Boolean

        Protected Friend scalarValue As INDArray

        ''' <summary>The dimensions of the op (Java field <c>dimensions</c>).</summary>
        Protected Friend dimensionsField() As Integer

        ''' <summary>The extra arguments of the op (Java field <c>extraArgs</c>).</summary>
        Protected Friend extraArgsField() As Object

        Protected Friend ownName As String

        Protected Friend ownNameSetWithDefault As Boolean = False

        Public Sub New()
            Me.New(True)
        End Sub

        Public Sub New(ByVal sameDiff As Boolean)
            'Only need instance ID if using function in context of SameDiff, not standard ND4J with INDArray args
            If sameDiff Then
                setInstanceId()
            End If
        End Sub

#If MODEL_IMPORT Then
		''' <summary>
		''' Initialize the function from the given
		''' <seealso cref="NodeDef"/> </summary>
		''' <param name="nodeDef"> </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal nodeDef As NodeDef, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Me.sameDiff = sameDiff
			setInstanceId()
			initFromTensorFlow(nodeDef, sameDiff,attributesForNode,graph)
		End Sub

		''' <summary>
		''' Initialize the function from the given
		''' <seealso cref="onnx.Onnx.NodeProto"/> </summary>
		''' <param name="node"> </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal node As Onnx.NodeProto, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Me.sameDiff = sameDiff
			setInstanceId()
			initFromOnnx(node, sameDiff, attributesForNode, graph)
		End Sub
#End If

        ''' <summary>
        ''' Returns the <see cref="AttributeAdapter"/>s for each of the
        ''' possible ops for import (typically tensorflow and onnx)
        ''' <para>
        ''' See <see cref="AttributeAdapter"/> for more information on what the
        ''' adapter does.
        ''' </para>
        ''' <para>
        ''' Similar to <see cref="mappingsForFunction()"/>, the returned map
        ''' contains a <see cref="AttributeAdapter"/> for each field name
        ''' when one is present. (It is optional for one to exist)
        ''' </para>
        ''' </summary>
        Public Overridable Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
            Return New Dictionary(Of String, IDictionary(Of String, AttributeAdapter))()
        End Function

        ''' <summary>
        ''' Returns the mappings for a given function (
        ''' for tensorflow and onnx import mapping properties
        ''' of this function). The mapping is indexed by field name.
        ''' If the function has no properties, this returned map
        ''' will be empty.
        ''' <para>
        ''' Note that some functions have multiple names.
        ''' This function returns a map indexed by each
        ''' alias it has for a given name.
        ''' These names include both onnx and tensorflow names (which might be 1 or more)
        ''' </para>
        ''' </summary>
        Public Overridable Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
            Return New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
        End Function

        ''' <summary>
        ''' Returns the properties for a given function. The fields come from the import machinery
        ''' (<c>DifferentialFunctionClassHolder</c>), so outside <c>#If MODEL_IMPORT</c> this throws
        ''' <see cref="NotSupportedException"/>; op classes may override it.
        ''' </summary>
        Public Overridable Function propertiesForFunction() As IDictionary(Of String, Object)
#If MODEL_IMPORT Then
			Dim fields As IDictionary(Of String, System.Reflection.FieldInfo) = DifferentialFunctionClassHolder.Instance.getFieldsForFunction(Me)
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			Preconditions.checkNotNull(fields, "DifferentialFunctionClassHolder returned null fields for %s - op has not been added to ImportClassMapping?", Me.GetType())

			For Each entry As val In fields.SetOfKeyValuePairs()
				Try
					ret(entry.getKey()) = fields(entry.getKey()).get(Me)
				Catch e As IllegalAccessException
					Throw New Exception("Unable to get property for field: " & entry.getKey(), e)
				End Try
			Next entry

			Return ret
#Else
            Throw New NotSupportedException("propertiesForFunction needs model import (phase 3, samediff-import)")
#End If
        End Function

        ''' <summary>
        ''' Sets the properties of this function from a map of field names to values. The fields come from the import
        ''' machinery (<c>DifferentialFunctionClassHolder</c>), so outside <c>#If MODEL_IMPORT</c> this throws
        ''' <see cref="NotSupportedException"/>; op classes may override it. (Java: the setter of
        ''' <c>propertiesForFunction</c>.)
        ''' </summary>
        Public Overridable Sub setPropertiesForFunction(ByVal properties As IDictionary(Of String, Object))
#If MODEL_IMPORT Then
				Dim fields As IDictionary(Of String, System.Reflection.FieldInfo) = DifferentialFunctionClassHolder.Instance.getFieldsForFunction(Me)
				For Each s As String In properties.Keys
					Dim f As System.Reflection.FieldInfo = fields(s)
					If f Is Nothing Then
	'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						log.warn("No fields found for property name {} for class {}", s, Me.GetType().FullName)
						Continue For
					End If
					setValueFor(f, properties(s))
				Next s
#Else
            Throw New NotSupportedException("setPropertiesForFunction needs model import (phase 3, samediff-import)")
#End If
        End Sub

        ''' <summary>
        ''' Get the value for a given property
        ''' for this function </summary>
        ''' <param name="property"> the property to get </param>
        ''' <returns> the value for the function if it exists </returns>
        Public Overridable Function getValue(ByVal [property] As FieldInfo) As Object
            Try
                Return [property].GetValue(Me)
            Catch e As FieldAccessException
                log.error("", e)
            End Try

            Return Nothing
        End Function

        ''' <summary>
        ''' Set the value for this function.
        ''' Note that if value is null an <see cref="ND4VBIllegalStateException"/>
        ''' will be thrown. </summary>
        ''' <param name="target"> the target field </param>
        ''' <param name="value"> the value to set </param>
        Public Overridable Sub setValueFor(ByVal target As FieldInfo, ByVal value As Object)
            If value Is Nothing AndAlso target.FieldType.IsPrimitive Then
                Throw New ND4VBIllegalStateException("Unable to set primitive field " & target.Name & " of type " & target.FieldType.FullName & " using null value!")
            End If

            If value IsNot Nothing Then
                value = ensureProperType(target, value)
            End If

            If ConfigProperties Then
                Dim propertyName As String = configFieldName()
                If propertyName Is Nothing Then
                    propertyName = "config"
                End If
                Const declared As BindingFlags = BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.DeclaredOnly
                Dim currClass As Type = CObj(Me).GetType()
                Dim f As FieldInfo = currClass.GetField(propertyName, declared)
                Do While f Is Nothing AndAlso currClass.BaseType IsNot Nothing
                    'OK, try superclass
                    currClass = currClass.BaseType
                    f = currClass.GetField(propertyName, declared)
                Loop

                If f Is Nothing Then
                    Throw New InvalidOperationException("Could not find field """ & propertyName & """ for class " & CObj(Me).GetType().FullName)
                End If

                Dim o As Object = f.GetValue(Me)
                If o Is Nothing Then
                    'Null config class - try to create one...
                    Dim c As Type = f.FieldType
                    Try
                        o = Activator.CreateInstance(c)
                    Catch e As System.Exception When TypeOf e Is MissingMethodException OrElse TypeOf e Is MemberAccessException OrElse TypeOf e Is TargetInvocationException
                        Throw New System.Exception("Error creating new instance of configuration object type " & c.FullName, e)
                    End Try
                    f.SetValue(Me, o)
                End If
                target.SetValue(o, value)
            Else
                'Edge case: we store float fields as doubles, rather than introduce an extra property
                If target.FieldType Is GetType(Single) AndAlso TypeOf value Is Double Then
                    value = CSng(DirectCast(value, Double))
                End If
                'Edge case: we store char fields as integers, rather than introduce an extra property
                If target.FieldType Is GetType(Char) AndAlso TypeOf value Is Integer Then
                    value = ChrW(DirectCast(value, Integer))
                End If

                target.SetValue(Me, value)
            End If
        End Sub

        ''' <summary>True for the boxed numeric types that stand in for Java's <c>Number</c>.</summary>
        Private Shared Function isNumber(ByVal value As Object) As Boolean
            Return TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Long OrElse
                TypeOf value Is Integer OrElse TypeOf value Is Short OrElse TypeOf value Is SByte OrElse
                TypeOf value Is Byte OrElse TypeOf value Is UShort OrElse TypeOf value Is UInteger OrElse
                TypeOf value Is ULong OrElse TypeOf value Is Decimal
        End Function

        ''' <summary>Java's <c>Number.longValue()</c>: floating values truncate toward zero and saturate, NaN gives 0.</summary>
        Private Shared Function longValue(ByVal value As Object) As Long
            If TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal Then
                Dim d As Double = Convert.ToDouble(value)
                If Double.IsNaN(d) Then Return 0L
                If d >= 9.2233720368547758E+18 Then Return Long.MaxValue
                If d <= -9.2233720368547758E+18 Then Return Long.MinValue
                Return CLng(Math.Truncate(d))
            End If
            If TypeOf value Is ULong Then
                Return CLng(DirectCast(value, ULong) And &H7FFFFFFFFFFFFFFFUL) Or If(DirectCast(value, ULong) > &H7FFFFFFFFFFFFFFFUL, Long.MinValue, 0L)
            End If
            Return Convert.ToInt64(value)
        End Function

        ''' <summary>Java's <c>Number.intValue()</c>: floating values truncate toward zero and saturate, integers wrap.</summary>
        Private Shared Function intValue(ByVal value As Object) As Integer
            If TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal Then
                Dim d As Double = Convert.ToDouble(value)
                If Double.IsNaN(d) Then Return 0
                If d >= Integer.MaxValue Then Return Integer.MaxValue
                If d <= Integer.MinValue Then Return Integer.MinValue
                Return CInt(Math.Truncate(d))
            End If
            Return Arrays.WrapToInt32(longValue(value))
        End Function

        Private Function ensureProperType(ByVal targetType As FieldInfo, ByVal value As Object) As Object
            Dim firstClass As Type = targetType.FieldType
            Dim valueType As Type = value.GetType()

            If Not firstClass.Equals(valueType) Then
                If firstClass.IsEnum Then
                    If valueType.Equals(GetType(String)) Then
                        For Each enumConstant As Object In [Enum].GetValues(firstClass)
                            If enumConstant.ToString().Equals(DirectCast(value, String), StringComparison.OrdinalIgnoreCase) Then
                                Return enumConstant
                            End If
                        Next enumConstant
                        Throw New InvalidOperationException("Could not find enum constant value for value """ & CStr(value) & """ for enum class " & firstClass.FullName)
                    End If
                ElseIf firstClass.Equals(GetType(Integer())) Then
                    If isNumber(value) Then
                        value = intValue(value)
                    End If
                    Return New Integer() {DirectCast(value, Integer)}
                ElseIf firstClass.Equals(GetType(Integer?())) Then
                    If isNumber(value) Then
                        value = intValue(value)
                    End If
                    Return New Integer?() {DirectCast(value, Integer)}
                ElseIf firstClass.Equals(GetType(Long())) Then
                    If isNumber(value) Then
                        value = longValue(value)
                    End If
                    Return New Long() {DirectCast(value, Long)}
                ElseIf firstClass.Equals(GetType(Long?())) Then
                    If isNumber(value) Then
                        value = longValue(value)
                    End If
                    Return New Long?() {DirectCast(value, Long)}
                ElseIf firstClass.Equals(GetType(Double())) Then
                    If isNumber(value) Then
                        value = Convert.ToDouble(value)
                    End If
                    Return New Double() {DirectCast(value, Double)}
                ElseIf firstClass.Equals(GetType(Double?())) Then
                    If isNumber(value) Then
                        value = Convert.ToDouble(value)
                    End If
                    Return New Double?() {DirectCast(value, Double)}
                ElseIf firstClass.Equals(GetType(Single())) Then
                    If isNumber(value) Then
                        value = Convert.ToSingle(value)
                    End If
                    Return New Single() {DirectCast(value, Single)}
                ElseIf firstClass.Equals(GetType(Single?())) Then
                    If isNumber(value) Then
                        value = Convert.ToSingle(value)
                    End If
                    Return New Single?() {DirectCast(value, Single)}
                End If
            End If

            Return value
        End Function

        ''' <summary>
        ''' Returns true if the fields for this class should be looked up from a configuration class.
        ''' (Java <c>isConfigProperties()</c>.)
        ''' </summary>
        Public Overridable ReadOnly Property ConfigProperties As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Returns the name of the field to be used for looking up field names.
        ''' This should be used in conjunction with <see cref="ConfigProperties"/>
        ''' to facilitate mapping fields for model import.
        ''' </summary>
        Public Overridable Function configFieldName() As String
            Return Nothing
        End Function

#If SAMEDIFF Then
		'''
		''' <param name="sameDiff"> </param>
		''' <param name="extraArgs"> </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inPlace As Boolean, ByVal extraArgs() As Object)
			Me.sameDiff = sameDiff
			Me.inPlace = inPlace
			setInstanceId()
			Me.extraArgs = extraArgs
		End Sub


		'''
		''' <param name="sameDiff"> </param>
		''' <param name="extraArgs"> </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal extraArgs() As Object)
			Me.sameDiff = sameDiff
			setInstanceId()
			Me.extraArgs = extraArgs
		End Sub

		'''
		''' <param name="sameDiff"> </param>
		''' <param name="args"> </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			Me.New(sameDiff,False, args)
		End Sub


		''' <summary>
		''' Add the various arguments for
		''' this function </summary>
		''' <param name="sameDiff"> </param>
		''' <param name="inPlace"> </param>
		''' <param name="args"> </param>
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inPlace As Boolean, ByVal args() As SDVariable)
			Me.sameDiff = sameDiff
			Me.inPlace = inPlace
			setInstanceId()
			If sameDiff IsNot Nothing AndAlso args IsNot Nothing Then
				sameDiff.addArgsFor(args, Me)
			End If
		End Sub

		Public Overridable Function getSameDiff() As SameDiff
			Return sameDiff
		End Function

		Public Overridable Sub setSameDiff(ByVal sameDiff As SameDiff)
			Me.sameDiff = sameDiff
		End Sub

		''' <summary>
		''' Replace argument at the specfied index </summary>
		''' <param name="i"> the index </param>
		''' <param name="newArg"> the new argument </param>
		Public Overridable Sub replaceArg(ByVal i As Integer, ByVal newArg As SDVariable)
			If sameDiff IsNot Nothing Then
				sameDiff.replaceArgFor(i, newArg, Me)
			End If
		End Sub


		''' <summary>
		''' Return the output variables for this differential function.
		''' Note that this op *may* dynamically generate variable outputs.
		''' @return
		''' </summary>
		Public Overridable Function outputVariables() As SDVariable()
			Return outputVariables(If(getOwnName() IsNot Nothing, getOwnName(), opName()))
		End Function

		''' <returns> The output variable, or the first output variable, if multiple outputs exist </returns>
		Public Overridable Function outputVariable() As SDVariable
			Return outputVariables()(0)
		End Function

		Public Overridable Function outputs() As IList(Of SDVariable)
			Dim [out]() As SDVariable = outputVariables()
			Return If([out] Is Nothing, Nothing, New List(Of SDVariable) From){[out]}
		End Function


		Public Overridable Function outputVariablesNames() As String()
			Dim outputVars() As SDVariable = outputVariables()
			Dim [out](outputVars.Length - 1) As String
			For i As Integer = 0 To [out].Length - 1
				[out](i) = outputVars(i).name()
			Next i
			Return [out]
		End Function


		''' <summary>
		''' Return the output functions for this differential function.
		''' @return
		''' </summary>
		Public MustOverride Function outputVariables(ByVal baseName As String) As SDVariable()



		''' <summary>
		''' The actual implementation for automatic differentiation.
		''' </summary>
		''' <param name="f1">
		''' @return </param>
		Public MustOverride Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)


		''' <summary>
		''' Return the arguments for a given function </summary>
		''' <returns> the arguments for a given function </returns>
		Public Overridable Function args() As SDVariable()
			Return If(sameDiff Is Nothing, Nothing, sameDiff.getInputVariablesForOp(Me))
		End Function

		''' <summary>
		''' Return the specified argument for this function </summary>
		''' <param name="num"> Number of the argument. Must be in range 0 to numArgs - 1 inclusive </param>
		''' <returns> Specified argument </returns>
		Public Overridable Function arg(ByVal num As Integer) As SDVariable
			Dim args() As SDVariable = Me.args()
			Preconditions.checkNotNull(args, "Arguments are null for function %s", Me.getOwnName())
			Preconditions.checkArgument(num >= 0 AndAlso num < args.Length, "Invalid index: must be 0 to numArgs (0 <= idx < %s), got %s", args.Length, num)
			Return args(num)
		End Function

		Public Overridable Function argNames() As String()
			Dim args() As SDVariable = Me.args()
			Dim [out](args.Length - 1) As String
			For i As Integer = 0 To args.Length - 1
				[out](i) = args(i).name()
			Next i
			Return [out]
		End Function

		''' <summary>
		''' Return the first argument
		''' @return
		''' </summary>
		Public Overridable Function arg() As SDVariable
			If args() Is Nothing OrElse args().Length = 0 Then
				Return Nothing
			End If
			Return args()(0)
		End Function


		''' <summary>
		''' Perform automatic differentiation
		''' wrt the input variables </summary>
		''' <param name="i_v1"> the input variables </param>
		''' <returns> the differentiated output
		''' wrt each input variable </returns>
		Public Overridable Function diff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim vals As IList(Of SDVariable) = doDiff(i_v1)
			If vals Is Nothing Then
				Throw New System.InvalidOperationException("Error executing diff operation: doDiff returned null for op: " & Me.opName())
			End If

			Dim outputVars As val = args()
			Dim copied As Boolean = False
			Dim i As Integer = 0
			Do While i < vals.Count
				Dim var As SDVariable = outputVars(i)
				Dim grad As SDVariable = If(var.hasGradient(), var.Gradient, Nothing)
				If grad IsNot Nothing Then
					If Not copied Then
						'Don't mutate the original - this could mess with the original op's state!
						vals = New List(Of SDVariable)(vals)
						copied = True
					End If

					Dim gradVar As SDVariable = var.getSameDiff().math.add(grad, vals(i))
					vals(i) = gradVar
					sameDiff.setGradientForVariableName(var.name(), gradVar)
				Else
					Dim gradVar As SDVariable = vals(i)

					sameDiff.updateVariableNameAndReference(gradVar,var.name() & "-grad")
					sameDiff.setGradientForVariableName(var.name(), gradVar)

				End If
				i += 1
			Loop

			Return vals
		End Function

		''' <summary>
		''' The left argument for this function
		''' @return
		''' </summary>
		Public Overridable Function larg() As SDVariable
			Dim args As val = Me.args()
			If args Is Nothing OrElse args.length = 0 Then
				Throw New ND4JIllegalStateException("No arguments found.")
			End If
			Return Me.args()(0)
		End Function

		''' <summary>
		''' The right argument for this function.
		''' Note that this assumes that there are 2 args for this
		''' function, if 2 are not set, it throws an
		''' <seealso cref="ND4JIllegalStateException"/>
		''' @return
		''' </summary>
		Public Overridable Function rarg() As SDVariable
			Dim args As val = Me.args()
			If args Is Nothing OrElse args.length <> 2 Then
				Throw New ND4JIllegalStateException("In order to use this function, the number of arguments for this function must be 2.")
			End If
			Return args(1)
		End Function
#End If

        ''' <summary>
        ''' Sets the own name if it is not set yet: inside SameDiff the graph's unique name for the op, otherwise a
        ''' random GUID.
        ''' </summary>
        Protected Friend Overridable Sub setInstanceId()
            If ownName Is Nothing Then
                ownNameSetWithDefault = True
#If SAMEDIFF Then
				If sameDiff Is Nothing Then
					Me.ownName = System.Guid.randomUUID().ToString()
				Else
					Dim n As String = sameDiff.getOpName(opName())
					Me.ownName = n
				End If

				If sameDiff IsNot Nothing Then
					sameDiff.putOpForId(ownName,Me)
				End If
#Else
                Me.ownName = Guid.NewGuid().ToString()
#End If
            End If
        End Sub

        ''' <summary>
        ''' The name of the op
        ''' </summary>
        Public Overridable Function opName() As String
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' The type of the op
        ''' </summary>
        Public Overridable Function opType() As Op.Type
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' The number of the op (mainly for old legacy XYZ ops
        ''' like <see cref="Op"/>)
        ''' </summary>
        Public Overridable Function opNum() As Integer
            Throw New NotSupportedException()
        End Function

        Public Overridable Function getInputArgument(ByVal index As Integer) As INDArray
            'Subclasses should implement this
            Throw New NotSupportedException("Not implemented")
        End Function

#If MODEL_IMPORT Then
		''' <summary>
		''' Initialize the function from the given
		''' <seealso cref="NodeDef"/> </summary>
		''' <param name="nodeDef"> </param>
		''' <param name="initWith"> </param>
		''' <param name="attributesForNode"> </param>
		''' <param name="graph"> </param>
		Public MustOverride Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		''' <summary>
		''' Iniitialize the function from the given
		''' <seealso cref="onnx.Onnx.NodeProto"/> </summary>
		''' <param name="node"> </param>
		''' <param name="initWith"> </param>
		''' <param name="attributesForNode"> </param>
		''' <param name="graph"> </param>
		Public MustOverride Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
#End If

        ''' <summary>
        ''' Duplicate this function. It clones through FlatBuffers serialization, so outside
        ''' <c>#If FLATBUFFERS</c> it throws <see cref="NotSupportedException"/>.
        ''' </summary>
        Public Overridable Function dup() As DifferentialFunction
#If FLATBUFFERS Then
			Return FlatBuffersMapper.cloneViaSerialize(sameDiff, Me)
#Else
            Throw New NotSupportedException("dup needs FlatBuffers serialization (phase 2 milestone M5)")
#End If
        End Function

        ''' <summary>
        ''' Calculate the output shape for this op </summary>
        ''' <returns> List of output shape descriptors </returns>
        Public Overridable Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Throw New ND4VBIllegalStateException("Op type of " & CObj(Me).GetType().FullName & "did not override calculateOutputShape() method leaked out for [" & Me.opName() & "]")
        End Function

        Public Overridable Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
            Throw New ND4VBIllegalStateException("Op type of " & CObj(Me).GetType().FullName & " did not override calculateOutputShape(OpContext) method leaked out for [" & Me.opName() & "]")
        End Function

        ''' <summary>
        ''' Calculate the data types for the output arrays.
        ''' Though datatypes can also be inferred from <see cref="calculateOutputShape()"/>, this method differs in that it does not
        ''' require the input arrays to be populated.
        ''' This is important as it allows us to do greedy datatype inference for the entire net - even if arrays are not
        ''' available.
        ''' </summary>
        ''' <param name="dataTypes"> The data types of the inputs </param>
        ''' <returns> The data types of the outputs </returns>
        Public Overridable Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("Op type of " & CObj(Me).GetType().FullName & " and name " & Me.ToString() & " did not override  calculateOutputDataTypes()! This function has not been implemented for " & CObj(Me).GetType().FullName)
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse CObj(Me).GetType() IsNot o.GetType() Then
                Return False
            End If

            Dim that As DifferentialFunction = DirectCast(o, DifferentialFunction)

            If inPlace <> that.inPlace Then
                Return False
            End If
            If If(scalarValue IsNot Nothing, Not scalarValue.Equals(that.scalarValue), that.scalarValue IsNot Nothing) Then
                Return False
            End If
            If Not Arrays.equals(dimensionsField, that.dimensionsField) Then
                Return False
            End If
            Return If(ownName IsNot Nothing, ownName.Equals(that.ownName), that.ownName Is Nothing)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 31
            result = 31 * result + (If(inPlace, 1, 0))
            result = 31 * result + (If(scalarValue IsNot Nothing, scalarValue.GetHashCode(), 0))
            result = 31 * result + Arrays.hashCode(dimensionsField)
            result = 31 * result + (If(ownName IsNot Nothing, ownName.GetHashCode(), 0))
            Return result
        End Function

        ''' <summary>
        ''' The opName of this function in onnx
        ''' </summary>
        Public Overridable Function onnxNames() As String()
            Return New String() {onnxName()}
        End Function

        ''' <summary>
        ''' The opName of this function tensorflow
        ''' </summary>
        Public Overridable Function tensorflowNames() As String()
            Return New String() {tensorflowName()}
        End Function

        ''' <summary>
        ''' The opName of this function in onnx
        ''' </summary>
        Public MustOverride Function onnxName() As String

        ''' <summary>
        ''' The opName of this function tensorflow
        ''' </summary>
        Public MustOverride Function tensorflowName() As String

        ''' <summary>The number of outputs of the op, or -1 when it is not known (Java <c>getNumOutputs()</c>).</summary>
        Public Overridable ReadOnly Property NumOutputs As Integer
            Get
                Return -1
            End Get
        End Property

        ''' <summary>
        ''' Clear the input and output INDArrays, if any are set
        ''' </summary>
        Public MustOverride Sub clearArrays()

        ' ---------------------------------------------------------------- Lombok accessors

        Public Overridable Function isInPlace() As Boolean
            Return inPlace
        End Function

        Public Overridable Sub setInPlace(ByVal inPlace As Boolean)
            Me.inPlace = inPlace
        End Sub

        Public Overridable Function getScalarValue() As INDArray
            Return scalarValue
        End Function

        Public Overridable Sub setScalarValue(ByVal scalarValue As INDArray)
            Me.scalarValue = scalarValue
        End Sub

        Public Overridable Function getDimensions() As Integer()
            Return dimensionsField
        End Function

        Public Overridable Sub setDimensions(ParamArray ByVal dimensions() As Integer)
            Me.dimensionsField = dimensions
        End Sub

        Public Overridable Function getExtraArgs() As Object()
            Return extraArgsField
        End Function

        Public Overridable Sub setExtraArgs(ByVal extraArgs() As Object)
            Me.extraArgsField = extraArgs
        End Sub

        Public Overridable Function getOwnName() As String
            Return ownName
        End Function

        Public Overridable Sub setOwnName(ByVal ownName As String)
            Me.ownName = ownName
        End Sub

        Public Overridable Function isOwnNameSetWithDefault() As Boolean
            Return ownNameSetWithDefault
        End Function

        Public Overridable Sub setOwnNameSetWithDefault(ByVal ownNameSetWithDefault As Boolean)
            Me.ownNameSetWithDefault = ownNameSetWithDefault
        End Sub

    End Class

End Namespace
