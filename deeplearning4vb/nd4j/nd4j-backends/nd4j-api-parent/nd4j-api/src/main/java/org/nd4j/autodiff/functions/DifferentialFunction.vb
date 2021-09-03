Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports FlatBuffersMapper = org.nd4j.autodiff.samediff.serde.FlatBuffersMapper
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports JsonIgnore = org.nd4j.shade.jackson.annotation.JsonIgnore
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

Namespace org.nd4j.autodiff.functions



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j public abstract class DifferentialFunction
	Public MustInherit Class DifferentialFunction
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @JsonIgnore protected org.nd4j.autodiff.samediff.SameDiff sameDiff;
		Protected Friend sameDiff As SameDiff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @JsonIgnore protected boolean inPlace;
		Protected Friend inPlace As Boolean



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @JsonIgnore protected org.nd4j.linalg.api.ndarray.INDArray scalarValue;
		Protected Friend scalarValue As INDArray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @JsonIgnore protected int[] dimensions;
		Protected Friend dimensions() As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore protected Object[] extraArgs;
		Protected Friend extraArgs() As Object


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @JsonIgnore protected String ownName;
		Protected Friend ownName As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore @Getter @Setter protected boolean ownNameSetWithDefault = false;
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


		''' <summary>
		''' Returns the <seealso cref="AttributeAdapter"/> s for each of the
		''' possible ops for import (typically tensorflow and onnx)
		''' 
		''' See <seealso cref="AttributeAdapter"/> for more information on what the
		''' adapter does.
		''' 
		''' Similar to <seealso cref="mappingsForFunction()"/>, the returned map
		''' contains a <seealso cref="AttributeAdapter"/> for each field name
		''' when one is present. (It is optional for one to exist)_
		''' @return
		''' </summary>
		Public Overridable Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Return java.util.Collections.emptyMap()
		End Function

		''' <summary>
		''' Returns the mappings for a given function (
		''' for tensorflow and onnx import mapping properties
		''' of this function). The mapping is indexed by field name.
		''' If the function has no properties, this returned map
		''' will be empty.
		''' 
		''' Note that some functions have multiple names.
		''' This function returns a map indexed by each
		''' alias it has for a given name.
		''' These names include both onnx and tensorflow names (which might be 1 or more)
		''' 
		''' @return
		''' </summary>
		Public Overridable Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Return java.util.Collections.emptyMap()
		End Function

		''' <summary>
		''' Returns the properties for a given function
		''' @return
		''' </summary>
		Public Overridable Function propertiesForFunction() As IDictionary(Of String, Object)
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
		End Function

		Public Overridable WriteOnly Property PropertiesForFunction As IDictionary(Of String, Object)
			Set(ByVal properties As IDictionary(Of String, Object))
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
			End Set
		End Property


		''' <summary>
		''' Get the value for a given property
		''' for this function </summary>
		''' <param name="property"> the property to get </param>
		''' <returns> the value for the function if it exists </returns>
		Public Overridable Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			Try
				Return [property].get(Me)
			Catch e As IllegalAccessException
				log.error("",e)
			End Try

			Return Nothing
		End Function

		''' <summary>
		''' Set the value for this function.
		''' Note that if value is null an <seealso cref="ND4JIllegalStateException"/>
		''' will be thrown. </summary>
		''' <param name="target"> the target field </param>
		''' <param name="value"> the value to set </param>
		Public Overridable Sub setValueFor(ByVal target As System.Reflection.FieldInfo, ByVal value As Object)
			If value Is Nothing AndAlso target.getType().isPrimitive() Then
				Throw New ND4JIllegalStateException("Unable to set primitive field " & target & " of type " & target.GetType() & " using null value!")
			End If

			If value IsNot Nothing Then
				value = ensureProperType(target, value)
			End If

			If ConfigProperties Then
				Dim propertyName As String = configFieldName()
				If propertyName Is Nothing Then
					propertyName = "config"
				End If
				Dim f As System.Reflection.FieldInfo = Nothing
				Dim currClass As Type = Me.GetType()
				Try
					f = currClass.getDeclaredField(propertyName)
				Catch e As NoSuchFieldException
					'OK, try superclass
				End Try
				Do While f Is Nothing AndAlso currClass.BaseType IsNot Nothing
					currClass = currClass.BaseType
					Try
						f = currClass.getDeclaredField(propertyName)
					Catch e As NoSuchFieldException
						'OK, try superclass
					End Try
				Loop

				If f Is Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.InvalidOperationException("Could not find field """ & propertyName & """ for class " & Me.GetType().FullName)
				End If

				Try
					f.setAccessible(True)
					Dim o As Object = f.get(Me)
					If o Is Nothing Then
						'Null config class - try to create one...
						Dim c As Type = f.getType()
						Try
							o = System.Activator.CreateInstance(c)
						Catch e As InstantiationException
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							Throw New Exception("Error creating new instance of configuration object type " & c.FullName, e)
						End Try
						f.set(Me, o)
					End If
					target.set(o, value)
				Catch e As IllegalAccessException
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New Exception("Error setting configuration field """ & propertyName & """ for config field """ & propertyName & """ on class " & Me.GetType().FullName)
				End Try

			Else
				Try
					'Edge case: we store float fields as doubles, rather than introduce an extra property
					If target.getType() = GetType(Single) AndAlso TypeOf value Is Double? Then
						value = DirectCast(value, Double?).Value
					End If
					'Edge case: we store char fields as integers, rather than introduce an extra property
					If target.getType() = GetType(Char) AndAlso TypeOf value Is Integer? Then
						value = CChar(DirectCast(value, Integer?).Value)
					End If

					target.set(Me,value)
				Catch e As IllegalAccessException
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New Exception("Error setting property for function " & Me.GetType().FullName, e)
				End Try
			End If
		End Sub


		Private Function ensureProperType(ByVal targetType As System.Reflection.FieldInfo, ByVal value As Object) As Object
			Dim firstClass As val = targetType.getType()
			Dim valueType As val = value.GetType()

			If Not firstClass.Equals(valueType) Then
				If firstClass.isEnum() Then
					If valueType.Equals(GetType(String)) Then
						Dim enumConstants() As Object = firstClass.getEnumConstants()
						For i As Integer = 0 To enumConstants.Length - 1
							If enumConstants(i).ToString().Equals(DirectCast(value, String), StringComparison.OrdinalIgnoreCase) Then
								Return enumConstants(i)
							End If
						Next i
						Throw New System.InvalidOperationException("Could not find enum constant value for value """ & value & """ for enum class " & firstClass.getName())
					End If
				ElseIf firstClass.Equals(GetType(Integer())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.intValue()
					End If

					Dim otherValue As Integer = DirectCast(value, Integer)
					Dim setValue() As Integer = {otherValue}
					Return setValue
				ElseIf firstClass.Equals(GetType(Integer?())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.intValue()
					End If

					Dim otherValue As Integer? = DirectCast(value, Integer?)
					Dim setValue() As Integer? = {otherValue}
					Return setValue
				ElseIf firstClass.Equals(GetType(Long())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.longValue()
					End If

					Dim otherValue As Long = DirectCast(value, Long)
					Dim setValue() As Long = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Long?())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.longValue()
					End If

					Dim otherValue As Long? = DirectCast(value, Long?)
					Dim setValue() As Long? = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Double())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.doubleValue()
					End If


					Dim otherValue As Double = DirectCast(value, Double)
					Dim setValue() As Double = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Double?())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.doubleValue()
					End If


					Dim otherValue As Double? = DirectCast(value, Double?)
					Dim setValue() As Double? = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Single())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.floatValue()
					End If


					Dim otherValue As Single = DirectCast(value, Single)
					Dim setValue() As Single = {otherValue}
					Return setValue

				ElseIf firstClass.Equals(GetType(Single?())) Then
					If TypeOf value Is Number Then
						Dim number As Number = DirectCast(value, Number)
						value = number.floatValue()
					End If



					Dim otherValue As Single? = DirectCast(value, Single?)
					Dim setValue() As Single? = {otherValue}
					Return setValue

				End If
			End If

			Return value
		End Function


		''' <summary>
		''' Returns true if the fields for this class should be looked up from a configuration class.
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ConfigProperties As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns the name of the field to be used for looking up field names.
		''' This should be used in conjunction with <seealso cref="isConfigProperties()"/>
		'''  to facilitate mapping fields for model import.
		''' @return
		''' </summary>
		Public Overridable Function configFieldName() As String
			Return Nothing
		End Function


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


		Protected Friend Overridable Sub setInstanceId()
			If ownName Is Nothing Then
				ownNameSetWithDefault = True
				If sameDiff Is Nothing Then
					Me.ownName = System.Guid.randomUUID().ToString()
				Else
					Dim n As String = sameDiff.getOpName(opName())
					Me.ownName = n
				End If

				If sameDiff IsNot Nothing Then
					sameDiff.putOpForId(ownName,Me)
				End If
			End If
		End Sub


		''' <summary>
		''' The name of the op
		''' @return
		''' </summary>
		Public Overridable Function opName() As String
			Throw New System.NotSupportedException()
		End Function


		''' <summary>
		''' The type of the op
		''' @return
		''' </summary>
		Public Overridable Function opType() As Op.Type
			Throw New System.NotSupportedException()
		End Function


		''' <summary>
		''' The number of the op (mainly for old legacy XYZ ops
		''' like <seealso cref="Op"/>)
		''' @return
		''' </summary>
		Public Overridable Function opNum() As Integer
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnore public org.nd4j.linalg.api.ndarray.INDArray getInputArgument(int index)
		Public Overridable Function getInputArgument(ByVal index As Integer) As INDArray
			'Subclasses should implement this
			Throw New System.NotSupportedException("Not implemented")
		End Function



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


		''' <summary>
		''' Duplicate this function
		''' @return
		''' </summary>
		Public Overridable Function dup() As DifferentialFunction
			Return FlatBuffersMapper.cloneViaSerialize(sameDiff, Me)
		End Function

		''' <summary>
		''' Calculate the output shape for this op </summary>
		''' <returns> List of output shape descriptors </returns>
		Public Overridable Function calculateOutputShape() As IList(Of LongShapeDescriptor)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New ND4JIllegalStateException("Op type of " & Me.GetType().FullName & "did not override calculateOutputShape() method leaked out for [" & Me.opName() & "]")
		End Function

		Public Overridable Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New ND4JIllegalStateException("Op type of " & Me.GetType().FullName & " did not override calculateOutputShape(OpContext) method leaked out for [" & Me.opName() & "]")
		End Function

		''' <summary>
		''' Calculate the data types for the output arrays.
		''' Though datatypes can also be inferred from <seealso cref="calculateOutputShape()"/>, this method differs in that it does not
		''' require the input arrays to be populated.
		''' This is important as it allows us to do greedy datatype inference for the entire net - even if arrays are not
		''' available.
		''' </summary>
		''' <param name="dataTypes"> The data types of the inputs </param>
		''' <returns> The data types of the outputs </returns>
		Public Overridable Function calculateOutputDataTypes(ByVal dataTypes As IList(Of org.nd4j.linalg.api.buffer.DataType)) As IList(Of org.nd4j.linalg.api.buffer.DataType)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Op type of " & Me.GetType().FullName & " and name " & Me.ToString() & " did not override  calculateOutputDataTypes()! This function has not been implemented for " & Me.GetType().FullName)
		End Function


		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As DifferentialFunction = DirectCast(o, DifferentialFunction)

			If inPlace <> that.inPlace Then
				Return False
			End If
			If If(scalarValue IsNot Nothing, Not scalarValue.Equals(that.scalarValue), that.scalarValue IsNot Nothing) Then
				Return False
			End If
			If Not dimensions.SequenceEqual(that.dimensions) Then
				Return False
			End If
			Return If(ownName IsNot Nothing, ownName.Equals(that.ownName), that.ownName Is Nothing)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 31
			result = 31 * result + (If(inPlace, 1, 0))
			result = 31 * result + (If(scalarValue IsNot Nothing, scalarValue.GetHashCode(), 0))
			result = 31 * result + java.util.Arrays.hashCode(dimensions)
			result = 31 * result + (If(ownName IsNot Nothing, ownName.GetHashCode(), 0))
			Return result
		End Function

		''' <summary>
		''' The opName of this function in onnx
		''' @return
		''' </summary>
		Public Overridable Function onnxNames() As String()
			Return New String() {onnxName()}
		End Function

		''' <summary>
		''' The opName of this function tensorflow
		''' 
		''' @return
		''' </summary>
		Public Overridable Function tensorflowNames() As String()
			Return New String() {tensorflowName()}
		End Function

		''' <summary>
		''' The opName of this function in onnx
		''' @return
		''' </summary>
		Public MustOverride Function onnxName() As String

		''' <summary>
		''' The opName of this function tensorflow
		''' 
		''' @return
		''' </summary>
		Public MustOverride Function tensorflowName() As String

		Public Overridable ReadOnly Property NumOutputs As Integer
			Get
				Return -1
			End Get
		End Property

		''' <summary>
		''' Clear the input and output INDArrays, if any are set
		''' </summary>
		Public MustOverride Sub clearArrays()
	End Class

End Namespace