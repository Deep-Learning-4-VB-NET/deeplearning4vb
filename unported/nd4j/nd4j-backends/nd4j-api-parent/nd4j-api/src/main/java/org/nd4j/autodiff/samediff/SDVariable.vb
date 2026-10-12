Imports System
Imports System.Collections.Generic
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports WeightInitScheme = org.nd4j.weightinit.WeightInitScheme

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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @Slf4j public class SDVariable implements java.io.Serializable
	<Serializable>
	Public Class SDVariable

'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Protected Friend sameDiff_Conflict As SameDiff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected String varName;
'JAVA TO VB CONVERTER NOTE: The field varName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend varName_Conflict As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected VariableType variableType;
		Protected Friend variableType As VariableType

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.NONE) protected long[] shape;
'JAVA TO VB CONVERTER NOTE: The field shape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend shape_Conflict() As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(AccessLevel.NONE) @Setter protected org.nd4j.linalg.api.buffer.DataType dataType;
'JAVA TO VB CONVERTER NOTE: The field dataType was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend dataType_Conflict As DataType

		Private creator As DifferentialFunction

		' autogen_tag::sdvars::start


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable(@NonNull String varName, @NonNull VariableType varType, @NonNull SameDiff sameDiff, long[] shape, org.nd4j.linalg.api.buffer.DataType dataType)
'JAVA TO VB CONVERTER NOTE: The parameter sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Sub New(ByVal varName As String, ByVal varType As VariableType, ByVal sameDiff_Conflict As SameDiff, ByVal shape() As Long, ByVal dataType As DataType)
			Preconditions.checkState(dataType <> DataType.UNKNOWN, "Unknown datatype is not allowed for SDVariables (variable name: %s)", varName)

			varName = sameDiff_Conflict.generateNewVarName(varName, 0, True)

			Me.sameDiff_Conflict = sameDiff_Conflict
			Me.varName_Conflict = varName
			Me.variableType = varType
			Me.dataType_Conflict = dataType
			Me.shape_Conflict = shape
		End Sub

		''' <summary>
		''' Get the name of the SDVariable </summary>
		''' <returns> Name of the variable </returns>
		Public Overridable Function name() As String
			Return varName_Conflict
		End Function

		Public Overridable Property VarName As String
			Set(ByVal varName As String)
				Me.varName_Conflict = varName
			End Set
			Get
				Return name()
			End Get
		End Property


		''' <summary>
		''' Returns true if this variable is a place holder
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property PlaceHolder As Boolean
			Get
				Return variableType = VariableType.PLACEHOLDER
			End Get
		End Property

		Public Overridable ReadOnly Property Constant As Boolean
			Get
				Return variableType = VariableType.CONSTANT
			End Get
		End Property

		''' <summary>
		''' A getter for the allocated ndarray with this <seealso cref="SDVariable"/>.
		''' 
		''' This getter will lazy initialize an array if one is not found based on the associated shape and
		''' <seealso cref="WeightInitScheme"/> - if this is possible. If this is not possible (due to shapes being unknown, etc)
		''' null is returned
		''' </summary>
		''' <returns> the <seealso cref="INDArray"/> associated with this variable. </returns>
		Public Overridable ReadOnly Property Arr As INDArray
			Get
				Return getArr(False)
			End Get
		End Property


		' autogen_tag::sdvars::end
		''' <summary>
		''' A getter for the allocated ndarray with this <seealso cref="SDVariable"/>.
		''' 
		''' This getter will lazy initialize an array if one is not found based on the associated shape and
		''' <seealso cref="WeightInitScheme"/> - if this is possible.<br>
		''' If this is not possible (due to shapes being unknown, etc) either:<br>
		''' (a) null is returned - if enforceExistence == false, or<br>
		''' (b) an IllegalStateException is thrown, if enforceExistence == true
		''' </summary>
		''' <returns> the <seealso cref="INDArray"/> associated with this variable. </returns>
		Public Overridable Function getArr(ByVal enforceExistence As Boolean) As INDArray
			If sameDiff_Conflict.arrayAlreadyExistsForVarName(VarName) Then
				Return sameDiff_Conflict.getArrForVarName(VarName)
			End If
			If variableType = VariableType.ARRAY Then
				Throw New System.NotSupportedException("Cannot get array for ARRAY type SDVariable - use SDVariable.exec or SameDiff.output instead")
			End If
			Dim ret As INDArray = sameDiff_Conflict.getArrForVarName(VarName)
			If enforceExistence AndAlso ret Is Nothing Then
				Throw New System.InvalidOperationException("No array exists for variable """ & name() & """")
			End If
			Return ret
		End Function


		''' <summary>
		''' Alias for the gradient variable - same as <seealso cref="getGradient()"/>.
		''' The gradient variable is the variable that represents the derivative of the loss function with respect
		''' to the output of this variable. I.e., if this variable is X and loss function is L, then gradient() returns the
		''' variable representing dL/dX.<br>
		''' Note that only floating point variables can have gradients.
		''' </summary>
		Public Overridable Function gradient() As SDVariable
			Return Gradient
		End Function

		''' <summary>
		''' The gradient variable is the variable that represents the derivative of the loss function with respect
		''' to the output of this variable. I.e., if this variable is X and loss function is L, then gradient() returns the
		''' variable representing dL/dX<br>
		''' Note that only floating point variables can have gradients.<br>
		''' Note also that a gradient may not yet be defined, and/or if no loss function variables have been set.<br>
		''' You can set the loss function variables using <seealso cref="SameDiff.setLossVariables(String...)"/> and then create the
		''' gradient functions using <seealso cref="SameDiff.createGradFunction()"/>. Alternatively, the gradient function will be
		''' created automatically when training is performed.
		''' </summary>
		Public Overridable ReadOnly Property Gradient As SDVariable
			Get
				Preconditions.checkState(dataType().isFPType(), "Cannot get gradient of %s datatype variable ""%s"": only floating" & " point variables have gradients", dataType(), VarName)
				Return sameDiff_Conflict.getGradForVariable(VarName)
			End Get
		End Property


		''' <summary>
		''' Returns the shape of this variable </summary>
		''' <returns> Shape of the variable </returns>
		Public Overridable Property Shape As Long()
			Get
				If variableType = VariableType.PLACEHOLDER Then
						Return shape_Conflict
				ElseIf variableType = VariableType.VARIABLE OrElse variableType = VariableType.CONSTANT Then
					Return Arr.shape()
				End If
    
				Return Nothing
			End Get
			Set(ByVal shape() As Long)
				Me.shape_Conflict = shape
			End Set
		End Property


		Public Overridable Function placeholderShape() As Long()
			If variableType <> VariableType.PLACEHOLDER Then
				Throw New System.InvalidOperationException("placeholderShape() can only be used for placeholder variables: variable """ & VarName & " is a variable of type " & variableType)
			End If
			Return shape_Conflict
		End Function

		Public Overridable Function dataType() As DataType
			If Me.dataType_Conflict = Nothing Then
				'Try to infer datatype instead of returning null
				If variableType <> VariableType.ARRAY AndAlso Arr IsNot Nothing Then
					Me.dataType_Conflict = Arr.dataType()
				End If
			End If

			Return Me.dataType_Conflict
		End Function

		Public Overridable ReadOnly Property ShapeDescriptor As LongShapeDescriptor
			Get
				Return LongShapeDescriptor.fromShape(Shape, Me.dataType())
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable castTo(@NonNull DataType dataType)
		Public Overridable Function castTo(ByVal dataType As DataType) As SDVariable
			Return castTo(Nothing, dataType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable castTo(String name, @NonNull DataType dataType)
		Public Overridable Function castTo(ByVal name As String, ByVal dataType As DataType) As SDVariable
			Return sameDiff_Conflict.castTo(name, Me, dataType)
		End Function


		''' <summary>
		''' Create a new SDVariable, the contents of which is copied from this current variable </summary>
		''' <returns> The new variable </returns>
		Public Overridable Function dup() As SDVariable
			Return sameDiff_Conflict.var(Me)
		End Function

		''' <summary>
		''' Return a variable with equal shape to the input, but all elements set to the specified value
		''' </summary>
		''' <param name="value"> Value for returned variable </param>
		''' <returns> new variable </returns>
		Public Overridable Function assign(ByVal value As Number) As SDVariable
			Return sameDiff_Conflict.scalarSet(Me, value.doubleValue())
		End Function

		''' <summary>
		''' Negate op - returns a new variable with the values of the current variable negated </summary>
		''' <returns> Negated variable </returns>
		Public Overridable Function neg() As SDVariable
			Return sameDiff_Conflict.math_Conflict.neg(Me)
		End Function

		''' <summary>
		''' Negate op - returns a new variable with the values of the current variable negated </summary>
		''' <param name="name"> Name of the new variable </param>
		''' <returns> Negated variable </returns>
		Public Overridable Function neg(ByVal name As String) As SDVariable
			Return sameDiff_Conflict.math().neg(name, Me)
		End Function

		''' <summary>
		''' See <seealso cref="lt(String, Double)"/>
		''' </summary>
		Public Overridable Function lt(ByVal value As Double) As SDVariable
			Return lt(Nothing, value)
		End Function

		''' <summary>
		''' Less than operation: elementwise {@code this < value}<br>
		''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or
		''' value 0 otherwise
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="value"> value argument to use in operation </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function lt(ByVal name As String, ByVal value As Double) As SDVariable
			Return sameDiff_Conflict.lt(name, Me, value)
		End Function

		''' <summary>
		''' See <seealso cref="lte(String, Double)"/>
		''' </summary>
		Public Overridable Function lte(ByVal value As Double) As SDVariable
			Return lte(Nothing, value)
		End Function

		''' <summary>
		''' Less than or equals operation: elementwise {@code this <= value}<br>
		''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or
		''' value 0 otherwise
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="value"> value argument to use in operation </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function lte(ByVal name As String, ByVal value As Double) As SDVariable
			Return sameDiff_Conflict.lte(name, Me, value)
		End Function

		''' <summary>
		''' See <seealso cref="gt(String, Double)"/>
		''' </summary>
		Public Overridable Function gt(ByVal value As Double) As SDVariable
			Return gt(Nothing, value)
		End Function

		''' <summary>
		''' Greater than operation: elementwise {@code this > value}<br>
		''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or
		''' value 0 otherwise
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="value"> value argument to use in operation </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function gt(ByVal name As String, ByVal value As Double) As SDVariable
			Return sameDiff_Conflict.gt(name, Me, value)
		End Function

		''' <summary>
		''' See <seealso cref="gte(String, Double)"/>
		''' </summary>
		Public Overridable Function gte(ByVal value As Double) As SDVariable
			Return gte(Nothing, value)
		End Function

		''' <summary>
		''' Greater than or equals operation: elementwise {@code this >= value}<br>
		''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or
		''' value 0 otherwise
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="value"> value argument to use in operation </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function gte(ByVal name As String, ByVal value As Double) As SDVariable
			Return sameDiff_Conflict.gte(name, Me, value)
		End Function

		''' <summary>
		''' See <seealso cref="eq(String, Double)"/>
		''' </summary>
		Public Overridable Function eq(ByVal value As Double) As SDVariable
			Return eq(Nothing, value)
		End Function

		''' <summary>
		''' Equals operation: elementwise {@code this == value}<br>
		''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or
		''' value 0 otherwise
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="value"> value argument to use in operation </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function eq(ByVal name As String, ByVal value As Double) As SDVariable
			Return sameDiff_Conflict.eq(name, Me, value)
		End Function

		''' <summary>
		''' See <seealso cref="neq(SDVariable)"/>
		''' </summary>
		Public Overridable Function neq(ByVal value As Double) As SDVariable
			Return neq(Nothing, value)
		End Function

		''' <summary>
		''' Not equals operation: elementwise {@code this != value}<br>
		''' Returns an array with the same shape/size as the input, with values 1 where condition is satisfied, or
		''' value 0 otherwise
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="value"> value argument to use in operation </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function neq(ByVal name As String, ByVal value As Double) As SDVariable
			Return sameDiff_Conflict.neq(name, Me, value)
		End Function


		''' <summary>
		''' See <seealso cref="lt(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function lt(ByVal other As SDVariable) As SDVariable
			Return lt(Nothing, other)
		End Function

		''' <summary>
		''' Less than operation: elementwise {@code this < y}<br>
		''' If x and y arrays have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if x and y have different shapes and are broadcastable, the output shape is broadcast.<br>
		''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="other"> Variable to compare values against </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function lt(ByVal name As String, ByVal other As SDVariable) As SDVariable
			Return sameDiff_Conflict.lt(name, Me, other)
		End Function

		''' <summary>
		''' See <seealso cref="lte(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function lte(ByVal other As SDVariable) As SDVariable
			Return lte(Nothing, other)
		End Function

		''' <summary>
		''' Less than or equal to operation: elementwise {@code this <= y}<br>
		''' If x and y arrays have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if x and y have different shapes and are broadcastable, the output shape is broadcast.<br>
		''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="other"> Variable to compare values against </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function lte(ByVal name As String, ByVal other As SDVariable) As SDVariable
			Return sameDiff_Conflict.lte(name, Me, other)
		End Function

		''' <summary>
		''' See <seealso cref="gt(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function gt(ByVal other As SDVariable) As SDVariable
			Return gt(Nothing, other)
		End Function

		''' <summary>
		''' Greater than operation: elementwise {@code this > y}<br>
		''' If x and y arrays have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if x and y have different shapes and are broadcastable, the output shape is broadcast.<br>
		''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="other"> Variable to compare values against </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function gt(ByVal name As String, ByVal other As SDVariable) As SDVariable
			Return sameDiff_Conflict.gt(name, Me, other)
		End Function

		''' <summary>
		''' See <seealso cref="gte(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function gte(ByVal other As SDVariable) As SDVariable
			Return gte(Nothing, other)
		End Function

		''' <summary>
		''' Greater than or equal to operation: elementwise {@code this >= y}<br>
		''' If x and y arrays have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if x and y have different shapes and are broadcastable, the output shape is broadcast.<br>
		''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="other"> Variable to compare values against </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function gte(ByVal name As String, ByVal other As SDVariable) As SDVariable
			Return sameDiff_Conflict.gte(name, Me, other)
		End Function

		''' <summary>
		''' See <seealso cref="eq(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function eq(ByVal other As SDVariable) As SDVariable
			Return eq(Nothing, other)
		End Function

		''' <summary>
		''' Equal to operation: elementwise {@code this == y}<br>
		''' If x and y arrays have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if x and y have different shapes and are broadcastable, the output shape is broadcast.<br>
		''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="other"> Variable to compare values against </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function eq(ByVal name As String, ByVal other As SDVariable) As SDVariable
			Return sameDiff_Conflict.eq(name, Me, other)
		End Function

		''' <summary>
		''' See <seealso cref="neq(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function neq(ByVal other As SDVariable) As SDVariable
			Return neq(Nothing, other)
		End Function

		''' <summary>
		''' Not equal to operation: elementwise {@code this != y}<br>
		''' If x and y arrays have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if x and y have different shapes and are broadcastable, the output shape is broadcast.<br>
		''' Returns an array with values 1 where condition is satisfied, or value 0 otherwise.
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="other"> Variable to compare values against </param>
		''' <returns> Output SDVariable with values 0 (not satisfied) and 1 (where the condition is satisfied) </returns>
		Public Overridable Function neq(ByVal name As String, ByVal other As SDVariable) As SDVariable
			Return sameDiff_Conflict.neq(name, Me, other)
		End Function

		''' <summary>
		''' See <seealso cref="mmul(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function mmul(ByVal other As SDVariable) As SDVariable
			Return mmul(Nothing, other)
		End Function

		''' <summary>
		''' Matrix multiplication: out = mmul(this,other)
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="other"> Other variable to perform matrix multiplication with </param>
		''' <returns> Output variable (result of mmul) </returns>
		Public Overridable Function mmul(ByVal name As String, ByVal other As SDVariable) As SDVariable
			Return sameDiff_Conflict.mmul(name, Me, other)
		End Function

		''' <summary>
		''' Matrix multiplication: out = mmul(this,other)
		''' </summary>
		''' <param name="name">          Name of the output variable </param>
		''' <param name="other">         Other variable to perform matrix multiplication with </param>
		''' <param name="mMulTranspose"> Matrix transpose configuration </param>
		''' <returns> Output variable (result of mmul) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable mmul(String name, SDVariable other, @NonNull MMulTranspose mMulTranspose)
		Public Overridable Function mmul(ByVal name As String, ByVal other As SDVariable, ByVal mMulTranspose As MMulTranspose) As SDVariable
			Return sameDiff_Conflict.mmul(name, Me, other, mMulTranspose.isTransposeA(), mMulTranspose.isTransposeB(), mMulTranspose.isTransposeResult())
		End Function


		''' <summary>
		''' See <seealso cref="dot(String, SDVariable, Integer...)"/>
		''' </summary>
		Public Overridable Function dot(ByVal other As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return dot(Nothing, other, dimensions)
		End Function

		''' <summary>
		''' Matrix dot product: out = dot(this,other, dimensions)
		''' </summary>
		''' <param name="name">  Name of the output variable </param>
		''' <param name="other"> Other variable to perform matrix multiplication with </param>
		''' <returns> Output variable (result of mmul) </returns>
		Public Overridable Function dot(ByVal name As String, ByVal other As SDVariable, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.dot(name, Me, other, dimensions)
		End Function



		''' <summary>
		''' See <seealso cref="add(String, Double)"/>
		''' </summary>
		Public Overridable Function add(ByVal scalar As Double) As SDVariable
			Return add(Nothing,scalar)
		End Function

		''' <summary>
		''' Scalar addition: {@code out = this + scalar}<br>
		''' Output variable has the same shape as the input variable
		''' </summary>
		''' <param name="varName"> Output variable name </param>
		''' <param name="scalar">  Scalar for operation </param>
		''' <returns> Output variable </returns>
		Public Overridable Function add(ByVal varName As String, ByVal scalar As Double) As SDVariable
			Dim [function] As val = sameDiff_Conflict.math_Conflict.add(Me,scalar)
			Return sameDiff_Conflict.updateVariableNameAndReference([function],varName)
		End Function

		''' <summary>
		''' See <seealso cref="add(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function add(ByVal other As SDVariable) As SDVariable
			Return add(Nothing,other)
		End Function

		''' <summary>
		''' Addition operation: elementwise {@code this + x}<br>
		''' If this and x variables have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if this and x have different shapes and are broadcastable, the output shape is broadcast.
		''' </summary>
		''' <param name="name"> Name of the output variable </param>
		''' <param name="x">    Variable to perform operation with </param>
		''' <returns> Output (result) SDVariable </returns>
		Public Overridable Function add(ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.add(Me, x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result, name)
		End Function

		''' <summary>
		''' For Kotlin operator interop </summary>
		''' <seealso cref= #add(String, SDVariable) </seealso>
		Public Overridable Function plus(ByVal other As SDVariable) As SDVariable
			Return add(other)
		End Function

		''' <summary>
		''' For Kotlin operator interop </summary>
		''' <seealso cref= #add(String, double) </seealso>
		Public Overridable Function plus(ByVal other As Double) As SDVariable
			Return add(other)
		End Function

		''' <summary>
		''' See <seealso cref="sub(String, Double)"/>
		''' </summary>
		Public Overridable Function [sub](ByVal scalar As Double) As SDVariable
			Return [sub](Nothing,scalar)
		End Function

		''' <summary>
		''' Scalar subtraction: {@code out = this - scalar}<br>
		''' Output variable has the same shape as the input variable
		''' </summary>
		''' <param name="varName"> Output variable name </param>
		''' <param name="scalar">  Scalar for operation </param>
		''' <returns> Output variable </returns>
		Public Overridable Function [sub](ByVal varName As String, ByVal scalar As Double) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.sub(Me, scalar)
			Return sameDiff_Conflict.updateVariableNameAndReference(result, varName)
		End Function

		''' <summary>
		''' See <seealso cref="sub(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function [sub](ByVal x As SDVariable) As SDVariable
			Return [sub](Nothing,x)
		End Function

		''' <summary>
		''' Subtraction operation: elementwise {@code this - x}<br>
		''' If this and x variables have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if this and x have different shapes and are broadcastable, the output shape is broadcast.
		''' </summary>
		''' <param name="name"> Name of the output variable </param>
		''' <param name="x">    Variable to perform operation with </param>
		''' <returns> Output (result) SDVariable </returns>
		Public Overridable Function [sub](ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.sub(Me,x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result,name)
		End Function

		''' <summary>
		''' For Kotlin operator interop </summary>
		''' <seealso cref= #sub(String, SDVariable) </seealso>
		Public Overridable Function minus(ByVal other As SDVariable) As SDVariable
			Return [sub](other)
		End Function

		''' <summary>
		''' For Kotlin operator interop </summary>
		''' <seealso cref= #sub(String, double) </seealso>
		Public Overridable Function minus(ByVal other As Double) As SDVariable
			Return [sub](other)
		End Function

		''' <summary>
		''' See <seealso cref="div(String,Double)"/>
		''' </summary>
		Public Overridable Function div(ByVal scalar As Double) As SDVariable
			Return div(Nothing,scalar)
		End Function

		''' <summary>
		''' Scalar division: {@code out = this / scalar}<br>
		''' Output variable has the same shape as the input variable
		''' </summary>
		''' <param name="varName"> Output variable name </param>
		''' <param name="scalar">  Scalar for operation </param>
		''' <returns> Output variable </returns>
		Public Overridable Function div(ByVal varName As String, ByVal scalar As Double) As SDVariable
			Dim [function] As val = sameDiff_Conflict.math_Conflict.div(Me,scalar)
			Return sameDiff_Conflict.updateVariableNameAndReference([function],varName)
		End Function

		''' <summary>
		''' See <seealso cref="div(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function div(ByVal x As SDVariable) As SDVariable
			Return div(Nothing,x)
		End Function

		''' <summary>
		''' Division operation: elementwise {@code this / x}<br>
		''' If this and x variables have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if this and x have different shapes and are broadcastable, the output shape is broadcast.
		''' </summary>
		''' <param name="name"> Name of the output variable </param>
		''' <param name="x">    Variable to perform operation with </param>
		''' <returns> Output (result) SDVariable </returns>
		Public Overridable Function div(ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.div(Me, x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result, name)
		End Function

		''' <summary>
		''' Floor division operation: elementwise {@code this // x}<br>
		''' If this and x variables have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if this and x have different shapes and are broadcastable, the output shape is broadcast.
		''' </summary>
		''' <param name="name"> Name of the output variable </param>
		''' <param name="x">    Variable to perform operation with </param>
		''' <returns> Output (result) SDVariable </returns>
		Public Overridable Function fdiv(ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.floorDiv(Me, x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result, name)
		End Function

		''' <summary>
		''' Modulo operation: elementwise {@code this / x}<br>
		''' If this and x variables have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if this and x have different shapes and are broadcastable, the output shape is broadcast.
		''' </summary>
		''' <param name="name"> Name of the output variable </param>
		''' <param name="x">    Variable to perform operation with </param>
		''' <returns> Output (result) SDVariable </returns>
		Public Overridable Function [mod](ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.mod(Me, x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result, name)
		End Function

		''' <summary>
		''' See <seealso cref="mul(String, Double)"/>
		''' </summary>
		Public Overridable Function mul(ByVal scalar As Double) As SDVariable
			Return mul(Nothing,scalar)
		End Function

		''' <summary>
		''' Scalar multiplication: {@code out = this * scalar}<br>
		''' Output variable has the same shape as the input variable
		''' </summary>
		''' <param name="varName"> Output variable name </param>
		''' <param name="scalar">  Scalar for operation </param>
		''' <returns> Output variable </returns>
		Public Overridable Function mul(ByVal varName As String, ByVal scalar As Double) As SDVariable
			Dim [function] As val = sameDiff_Conflict.math_Conflict.mul(Me, scalar)
			Return sameDiff_Conflict.updateVariableNameAndReference([function],varName)
		End Function


		''' <summary>
		''' See <seealso cref="mul(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function mul(ByVal x As SDVariable) As SDVariable
			Return mul(Nothing,x)
		End Function

		''' <summary>
		''' Multiplication operation: elementwise {@code this * x}<br>
		''' If this and x variables have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if this and x have different shapes and are broadcastable, the output shape is broadcast.
		''' </summary>
		''' <param name="name"> Name of the output variable </param>
		''' <param name="x">    Variable to perform operation with </param>
		''' <returns> Output (result) SDVariable </returns>
		Public Overridable Function mul(ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.mul(Me, x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result,name)
		End Function

		''' <summary>
		''' For Kotlin operator interop </summary>
		''' <seealso cref= #mul(String, SDVariable) </seealso>
		Public Overridable Function times(ByVal other As SDVariable) As SDVariable
			Return mul(other)
		End Function

		''' <summary>
		''' For Kotlin operator interop </summary>
		''' <seealso cref= #mul(String, double) </seealso>
		Public Overridable Function times(ByVal other As Double) As SDVariable
			Return mul(other)
		End Function

		''' <summary>
		''' See <seealso cref="pow(String, Double)"/>
		''' </summary>
		Public Overridable Function pow(ByVal scalar As Double) As SDVariable
			Return pow(Nothing, scalar)
		End Function

		''' <summary>
		''' Scalar power operation: {@code out = this ^ scalar}<br>
		''' Output variable has the same shape as the input variable
		''' </summary>
		''' <param name="varName"> Output variable name </param>
		''' <param name="scalar">  Scalar for operation </param>
		''' <returns> Output variable </returns>
		Public Overridable Function pow(ByVal varName As String, ByVal scalar As Double) As SDVariable
			Dim ret As SDVariable = sameDiff_Conflict.math_Conflict.pow(Me, scalar)
			Return sameDiff_Conflict.updateVariableNameAndReference(ret, varName)
		End Function

		''' <summary>
		''' See <seealso cref="rsub(String, Double)"/>
		''' </summary>
		Public Overridable Function rsub(ByVal scalar As Double) As SDVariable
			Return rsub(Nothing,scalar)
		End Function

		''' <summary>
		''' Scalar reverse subtraction: {@code out = scalar - this}<br>
		''' Output variable has the same shape as the input variable
		''' </summary>
		''' <param name="varName"> Output variable name </param>
		''' <param name="scalar">  Scalar for operation </param>
		''' <returns> Output variable </returns>
		Public Overridable Function rsub(ByVal varName As String, ByVal scalar As Double) As SDVariable
			Dim [function] As val = sameDiff_Conflict.math_Conflict.rsub(Me,scalar)
			Return sameDiff_Conflict.updateVariableNameAndReference([function],varName)
		End Function

		''' <summary>
		''' See <seealso cref="rsub(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function rsub(ByVal x As SDVariable) As SDVariable
			Return rsub(Nothing,x)
		End Function

		''' <summary>
		''' Reverse subtraction operation: elementwise {@code x - this}<br>
		''' If this and x variables have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if this and x have different shapes and are broadcastable, the output shape is broadcast.
		''' </summary>
		''' <param name="name"> Name of the output variable </param>
		''' <param name="x">    Variable to perform operation with </param>
		''' <returns> Output (result) SDVariable </returns>
		Public Overridable Function rsub(ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.rsub(Me,x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result,name)
		End Function

		''' <summary>
		''' See <seealso cref="rdiv(String, Double)"/>
		''' </summary>
		Public Overridable Function rdiv(ByVal scalar As Double) As SDVariable
			Return rdiv(Nothing,scalar)
		End Function

		''' <summary>
		''' Scalar reverse division: {@code out = scalar / this}<br>
		''' Output variable has the same shape as the input variable
		''' </summary>
		''' <param name="varName"> Output variable name </param>
		''' <param name="scalar">  Scalar for operation </param>
		''' <returns> Output variable </returns>
		Public Overridable Function rdiv(ByVal varName As String, ByVal scalar As Double) As SDVariable
			Dim [function] As val = sameDiff_Conflict.math_Conflict.rdiv(Me, scalar)
			Return sameDiff_Conflict.updateVariableNameAndReference([function], varName)
		End Function

		''' <summary>
		''' See <seealso cref="rdiv(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function rdiv(ByVal sameDiffVariable As SDVariable) As SDVariable
			Return rdiv(Nothing,sameDiffVariable)
		End Function

		''' <summary>
		''' Reverse division operation: elementwise {@code x / this}<br>
		''' If this and x variables have equal shape, the output shape is the same as the inputs.<br>
		''' Supports broadcasting: if this and x have different shapes and are broadcastable, the output shape is broadcast.
		''' </summary>
		''' <param name="name"> Name of the output variable </param>
		''' <param name="x">    Variable to perform operation with </param>
		''' <returns> Output (result) SDVariable </returns>
		Public Overridable Function rdiv(ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math_Conflict.rdiv(Me,x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result,name)

		End Function

		''' <summary>
		''' See <seealso cref="squaredDifference(String, SDVariable)"/>
		''' </summary>
		Public Overridable Function squaredDifference(ByVal x As SDVariable) As SDVariable
			Return squaredDifference(Nothing,x)
		End Function

		''' <summary>
		''' Squared difference operation: {@code (this - x)^2} </summary>
		''' <param name="x"> Other input variable </param>
		''' <returns> squared difference between variables </returns>
		Public Overridable Function squaredDifference(ByVal name As String, ByVal x As SDVariable) As SDVariable
			Dim result As val = sameDiff_Conflict.math().squaredDifference(Me, x)
			Return sameDiff_Conflict.updateVariableNameAndReference(result, name)
		End Function

		''' <summary>
		''' See <seealso cref="sum(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function sum(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sum(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="sum(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function sum(ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sum(Nothing, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="sum(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function sum(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sum(name, False, dimensions)
		End Function

		''' <summary>
		''' Sum array reduction operation, optionally along specified dimensions.<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Output variable name </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed </param>
		''' <returns> Output variable: reduced array of rank (input rank - num dimensions) if keepDims = false, or
		''' of rank (input rank) if keepdims = true </returns>
		Public Overridable Function sum(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.sum(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="mean(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function mean(ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return mean(Nothing, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="mean(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function mean(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return mean(name, False, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="mean(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function mean(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return mean(Nothing, False, dimensions)
		End Function


		''' <summary>
		''' Mean (average) array reduction operation, optionally along specified dimensions<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Output variable name </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed </param>
		''' <returns> Reduced array of rank (input rank - num dimensions) </returns>
		Public Overridable Function mean(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.mean(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="std(String, Boolean, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function std(ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return std(Nothing, biasCorrected, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="std(String, Boolean, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function std(ByVal name As String, ByVal biasCorrected As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.standardDeviation(name, Me, biasCorrected, dimensions)
		End Function

		''' <summary>
		''' Stardard deviation array reduction operation, optionally along specified dimensions<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="biasCorrected"> If true: divide by (N-1) (i.e., sample stdev). If false: divide by N (population stdev) </param>
		''' <param name="keepDims">      If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions">    Dimensions to reduce over. If dimensions are not specified, full array reduction is performed </param>
		''' <returns> Output variable: reduced array of rank (input rank - num dimensions) </returns>
		Public Overridable Function std(ByVal name As String, ByVal biasCorrected As Boolean, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.standardDeviation(name, Me, biasCorrected, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="prod(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function prod(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return prod(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="prod(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function prod(ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return prod(Nothing, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="prod(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function prod(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.prod(name, Me, dimensions)
		End Function

		''' <summary>
		''' Product array reduction operation, optionally along specified dimensions<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Output variable name </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed </param>
		''' <returns> Output variable: reduced array of rank (input rank - num dimensions) </returns>
		Public Overridable Function prod(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.prod(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="min(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function min(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return min(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="min(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function min(ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return min(Nothing, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="min(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function min(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return min(name, False, dimensions)
		End Function

		''' <summary>
		''' Minimum array reduction operation, optionally along specified dimensions. out = min(in)<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Output variable name </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed </param>
		''' <returns> Reduced array of rank (input rank - num dimensions) </returns>
		Public Overridable Function min(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.min(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="max(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function max(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return max(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="max(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function max(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return max(name, False, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="max(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function max(ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return max(Nothing, keepDims, dimensions)
		End Function

		''' <summary>
		''' Maximum array reduction operation, optionally along specified dimensions<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Output variable name </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed </param>
		''' <returns> Reduced array of rank (input rank - num dimensions) </returns>
		Public Overridable Function max(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.max(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="norm1(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function norm1(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return norm1(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="norm1(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function norm1(ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return norm1(Nothing, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="norm1(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function norm1(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return norm1(name, False, dimensions)
		End Function

		''' <summary>
		''' Norm1 (L1 norm) reduction operation: The output contains the L1 norm for each tensor/subset along the specified dimensions:<br>
		''' {@code out = sum_i abs(x[i])}<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Output variable name </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> dimensions to reduce over </param>
		''' <returns> Output variable </returns>
		Public Overridable Function norm1(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.norm1(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="norm2(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function norm2(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return norm2(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="norm2(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function norm2(ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return norm2(Nothing, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="norm2(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function norm2(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return norm2(name, False, dimensions)
		End Function

		''' <summary>
		''' Norm2 (L2 norm) reduction operation: The output contains the L2 norm for each tensor/subset along the specified dimensions:<br>
		''' {@code out = sqrt(sum_i x[i]^2)}<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Output variable name </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> dimensions to reduce over </param>
		''' <returns> Output variable </returns>
		Public Overridable Function norm2(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.norm2(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="normmax(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function normmax(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return normmax(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="normmax(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function normmax(ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return normmax(Nothing, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="normmax(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function normmax(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return normmax(name, False, dimensions)
		End Function

		''' <summary>
		''' Max norm (infinity norm) reduction operation: The output contains the max norm for each tensor/subset along the
		''' specified dimensions:<br>
		''' {@code out = max(abs(x[i]))}<br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Output variable name </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> dimensions to reduce over </param>
		''' <returns> Output variable </returns>
		Public Overridable Function normmax(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.normmax(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="argmax(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function argmax(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return argmax(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="argmax(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function argmax(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.argmax(name, Me, dimensions)
		End Function

		''' <summary>
		''' Argmax array reduction operation, optionally along specified dimensions.<br>
		''' Output values are the index of the maximum value of each slice along the specified dimension.<br>
		''' <br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Name of the output variable </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as size 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed </param>
		''' <returns> Output variable: reduced array of rank (input rank - num dimensions) if keepDims = false, or
		''' of rank (input rank) if keepdims = true </returns>
		Public Overridable Function argmax(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.argmax(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="argmin(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function argmin(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return argmin(Nothing, dimensions)
		End Function

		''' <summary>
		''' See <seealso cref="argmin(String, Boolean, Integer...)"/>
		''' </summary>
		Public Overridable Function argmin(ByVal name As String, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.argmin(name, Me, dimensions)
		End Function

		''' <summary>
		''' Argmin array reduction operation, optionally along specified dimensions.<br>
		''' Output values are the index of the minimum value of each slice along the specified dimension.<br>
		''' <br>
		''' Note that if keepDims = true, the output variable has the same rank as the input variable,
		''' with the reduced dimensions having size 1. This can be useful for later broadcast operations (such as subtracting
		''' the mean along a dimension).<br>
		''' Example: if input has shape [a,b,c] and dimensions=[1] then output has shape:
		''' keepDims = true: [a,1,c]<br>
		''' keepDims = false: [a,c]
		''' </summary>
		''' <param name="name">       Name of the output variable </param>
		''' <param name="keepDims">   If true: keep the dimensions that are reduced on (as length 1). False: remove the reduction dimensions </param>
		''' <param name="dimensions"> Dimensions to reduce over. If dimensions are not specified, full array reduction is performed </param>
		''' <returns> Output variable: reduced array of rank (input rank - num dimensions) if keepDims = false, or
		''' of rank (input rank) if keepdims = true </returns>
		Public Overridable Function argmin(ByVal name As String, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.argmax(name, Me, keepDims, dimensions)
		End Function

		''' <summary>
		''' Get the shape of the array as a dynamic SDVariable </summary>
		''' <returns> Shape SDVariable </returns>
		Public Overridable Function shape() As SDVariable
			Return sameDiff_Conflict.shape(Me)
		End Function

		''' <summary>
		''' Get the rank of this variable as a dynamic SDVariable </summary>
		''' <returns> Rank SDVariable </returns>
		Public Overridable Function rank() As SDVariable
			Return sameDiff_Conflict.rank(Me)
		End Function

		''' <summary>
		''' Reshape the current variable to the specified (dynamic) shape. The output variable will have the same values as the
		''' input, but with the specified shape.<br>
		''' Note that prod(shape) must match length(input) == prod(input.shape)
		''' </summary>
		''' <param name="newShape"> New shape for variable </param>
		''' <returns> Output variable </returns>
		Public Overridable Function reshape(ByVal newShape As SDVariable) As SDVariable
			Return sameDiff_Conflict.reshape(Me, newShape)
		End Function

		''' <summary>
		''' Reshape the current variable to the specified shape. The output variable will have the same values as the
		''' input, but with the specified shape.<br>
		''' Note that prod(shape) must match length(input) == prod(input.shape)
		''' </summary>
		''' <param name="newShape"> New shape for variable </param>
		''' <returns> Output variable </returns>
		Public Overridable Function reshape(ParamArray ByVal newShape() As Integer) As SDVariable
			Return sameDiff_Conflict.reshape(Me, ArrayUtil.toLongArray(newShape))
		End Function

		''' <summary>
		''' Reshape the current variable to the specified shape. The output variable will have the same values as the
		''' input, but with the specified shape.<br>
		''' Note that prod(shape) must match length(input) == prod(input.shape)
		''' </summary>
		''' <param name="newShape"> New shape for variable </param>
		''' <returns> Output variable </returns>
		Public Overridable Function reshape(ParamArray ByVal newShape() As Long) As SDVariable
			Return sameDiff_Conflict.reshape(Me, newShape)
		End Function

		''' <summary>
		''' Permute the dimensions of the current variable according to the specified permutation indices.<br>
		''' Example: if the current variable has shape [a,b,c] and dimensions = [2,0,1] the output has shape [c,a,b]
		''' </summary>
		''' <param name="dimensions"> The new dimension order </param>
		''' <returns> Output variable (permuted input) </returns>
		Public Overridable Function permute(ParamArray ByVal dimensions() As Integer) As SDVariable
			Return sameDiff_Conflict.permute(Me, dimensions)
		End Function

		Public Overridable Function permute(ByVal dimensions As SDVariable) As SDVariable
			Return sameDiff_Conflict.permute(Me, dimensions)
		End Function

		''' <summary>
		''' Associate the specified array with this variable </summary>
		''' <param name="array"> Array to associate with this variable </param>
		''' <returns> This variable </returns>
		Public Overridable Function setArray(ByVal array As INDArray) As SDVariable
			sameDiff_Conflict.associateArrayWithVariable(array, Me)
			Return Me
		End Function


		''' <summary>
		''' Evaluate the result of this variable
		''' @return
		''' </summary>
		Public Overridable Function eval() As INDArray
			Dim m As IDictionary(Of String, INDArray) = sameDiff_Conflict.output(DirectCast(Nothing, IDictionary(Of String, INDArray)), name())
			Return m(name())
		End Function


		''' <summary>
		''' Evaluate the result of this variable
		''' @return
		''' </summary>
		Public Overridable Function eval(ByVal placeholders As IDictionary(Of String, INDArray)) As INDArray
			Dim m As IDictionary(Of String, INDArray) = sameDiff_Conflict.output(placeholders, name())
			Return m(name())
		End Function


		Public Overrides Function ToString() As String
			Return "SDVariable(name=""" & varName_Conflict & """,variableType=" & variableType & ",dtype=" & dataType_Conflict + (If(variableType = VariableType.PLACEHOLDER AndAlso shape_Conflict IsNot Nothing, ",shape=" & Arrays.toString(shape_Conflict), "")) & ")"
		End Function

		''' <summary>
		''' Add a control dependency for this variable on the specified variable.<br>
		''' Control dependencies can be used to enforce the execution order.
		''' For example, if a control dependency X->Y exists, then Y will only be executed after X is executed - even
		''' if Y wouldn't normally depend on the result/values of X.
		''' </summary>
		''' <param name="controlDependency"> Control dependency to add for this variable </param>
		Public Overridable Sub addControlDependency(ByVal controlDependency As SDVariable)
			Dim vThis As Variable = sameDiff_Conflict.getVariables().get(VarName)
			Dim vCD As Variable = sameDiff_Conflict.getVariables().get(controlDependency.name())

			'If possible: add control dependency on ops
			If vThis.getOutputOfOp() IsNot Nothing AndAlso vCD.getOutputOfOp() IsNot Nothing Then
				'Op -> Op case
				Dim oThis As SameDiffOp = sameDiff_Conflict.getOps().get(vThis.getOutputOfOp())
				Dim oCD As SameDiffOp = sameDiff_Conflict.getOps().get(vCD.getOutputOfOp())

				If oThis.getControlDeps() Is Nothing Then
					oThis.ControlDeps = New List(Of String)()
				End If
				If Not oThis.getControlDeps().Contains(oCD.Name) Then
					oThis.getControlDeps().Add(oCD.Name)
				End If

				If oCD.getControlDepFor() Is Nothing Then
					oCD.ControlDepFor = New List(Of String)()
				End If
				If Not oCD.getControlDepFor().Contains(oThis.Name) Then
					oCD.getControlDepFor().Add(oThis.Name)
				End If
			Else
				If vThis.getOutputOfOp() IsNot Nothing Then
					'const/ph -> op case
					Dim oThis As SameDiffOp = sameDiff_Conflict.getOps().get(vThis.getOutputOfOp())

					If oThis.getVarControlDeps() Is Nothing Then
						oThis.VarControlDeps = New List(Of String)()
					End If

					If Not oThis.getVarControlDeps().Contains(vCD.getName()) Then
						oThis.getVarControlDeps().Add(vCD.getName())
					End If

					If vCD.getControlDepsForOp() Is Nothing Then
						vCD.setControlDepsForOp(New List(Of String)())
					End If
					If Not vCD.getControlDepsForOp().contains(oThis.Name) Then
						vCD.getControlDepsForOp().add(oThis.Name)
					End If
				Else
					'const/ph -> const/ph case
					If vThis.getControlDeps() Is Nothing Then
						vThis.setControlDeps(New List(Of String)())
					End If
					If Not vThis.getControlDeps().contains(vCD.getName()) Then
						vThis.getControlDeps().add(vCD.getName())
					End If

					If vCD.getControlDepsForVar() Is Nothing Then
						vCD.setControlDepsForVar(New List(Of String)())
					End If
					If Not vCD.getControlDepsForVar().contains(vThis.getName()) Then
						vCD.getControlDepsForVar().add(vThis.getName())
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Get a variable with content equal to a specified sub-array of this variable.<br>
		''' Can be used (for example) to get rows, columns, sub-matrices, etc. </summary>
		''' <param name="indices"> Indices to get </param>
		''' <returns> Sub-array variable </returns>
		Public Overridable Function get(ParamArray ByVal indices() As SDIndex) As SDVariable
			Dim ndims As Integer = indices.Length
			Dim begin(ndims - 1) As Long
			Dim [end](ndims - 1) As Long
			Dim strides(ndims - 1) As Long
			Dim begin_mask_arr(ndims - 1) As Integer
			Dim end_mask_arr(ndims - 1) As Integer
			Dim shrink_axis_mask_arr(ndims - 1) As Integer
			For i As Integer = 0 To ndims - 1
				strides(i) = 1
				Dim index As SDIndex = indices(i)
				Dim indexType As SDIndex.IndexType = index.getIndexType()
				If indexType = SDIndex.IndexType.ALL Then
					begin_mask_arr(i) = 1
					end_mask_arr(i) = 1
				ElseIf indexType = SDIndex.IndexType.POINT Then
					Dim pointIndex As Long = index.getPointIndex()
					begin(i) = pointIndex
					[end](i) = pointIndex + 1
					If Not index.isPointKeepDim() Then
						shrink_axis_mask_arr(i) = 1
					End If
				ElseIf indexType = SDIndex.IndexType.INTERVAL Then
					If index.getIntervalBegin() Is Nothing Then
						begin_mask_arr(i) = 1
					Else
						begin(i) = index.getIntervalBegin()
					End If
					If index.getIntervalEnd() Is Nothing Then
						end_mask_arr(i) = 1
					Else
						[end](i) = index.getIntervalEnd()
					End If
					If index.getIntervalStrides() Is Nothing Then
						strides(i) = 1
					Else
						strides(i) = index.getIntervalStrides()
					End If
				End If
			Next i

			' convert binary int[] to int
			Dim begin_mask As Integer = binArrToInt(begin_mask_arr)
			Dim end_mask As Integer = binArrToInt(end_mask_arr)
			Dim shrink_axis As Integer = binArrToInt(shrink_axis_mask_arr)

			Return Me.sameDiff_Conflict.stridedSlice(Me, begin, [end], strides, begin_mask, end_mask, 0, 0, shrink_axis)
		End Function

		''' <summary>
		''' Convert this variable to a constant. This is equivalent to "freezing" a variable so that it's value
		''' won't be changed by further training.<br>
		''' This can only be done for variables and placeholders, not ARRAY type variables (which are usually network activations).
		''' As a constant, this variable will no longer be modified by any subsequent training.
		''' </summary>
		''' <returns> This variable (now a constant) </returns>
		Public Overridable Function convertToConstant() As SDVariable
			Return sameDiff_Conflict.convertToConstant(Me)
		End Function

		''' <summary>
		''' Convert this variable to a VARIABLE type SDVariable.<br>
		''' This can only be done for constants and placeholders, not ARRAY type variables (which are usually network activations).
		''' As a variable, this variable will modified during any subsequent training.
		''' </summary>
		''' <returns> This variable (now a variable type SDVariable) </returns>
		Public Overridable Function convertToVariable() As SDVariable
			Return sameDiff_Conflict.convertToVariable(Me)
		End Function

		''' <summary>
		''' Rename this variable to a new name. Equivalent to <seealso cref="SameDiff.renameVariable(String, String)"/>
		''' </summary>
		''' <param name="newName"> The new name for the variable - no variable with this name must already exist </param>
		''' <returns> The current variable (same object) </returns>
		Public Overridable Function rename(ByVal newName As String) As SDVariable
			sameDiff_Conflict.renameVariable(VarName, newName)
			Return Me
		End Function

		''' <summary>
		''' Mark this variable as a loss function variable. This means that this variable will be minimized via backprop during training.<br>
		''' This will add the variable as a loss to any others - i.e., if multiple variables are marked as losses, their values will be summed
		''' to give the total network loss.<br>
		''' Note that only floating point (Float16/32/64) variables may be marked as a loss.<br>
		''' Note also that only ARRAY type SDVariables can be marked as losses to be minimized. That is, we cannot mark the value
		''' of a constant, variable or placeholder to be minimized as doing so would not make sense.<br>
		''' This is equivalent to <seealso cref="SameDiff.addLossVariable(String)"/>
		''' </summary>
		Public Overridable Sub markAsLoss()
			sameDiff_Conflict.addLossVariable(VarName)
		End Sub

		''' <summary>
		''' Determine if this variable has a gradient with respect to the current loss. Note that:
		''' (a) Non-floating-point variables (integer, string, etc) will never have gradients<br>
		''' (b) This method will return false if no gradient function has been created yet. See <seealso cref="SameDiff.createGradFunction()"/>
		''' and <seealso cref="SameDiff.setLossVariables(String...)"/><br>
		''' (c) Floating point variables may not have any gradient if the current loss does not depend on the variable at all<br> </summary>
		''' <returns> True if a gradient variable exists for the specified variable, for the current loss </returns>
		Public Overridable Function hasGradient() As Boolean
			Return sameDiff_Conflict.variableHasGradient(VarName)
		End Function

		Private Shared Function binArrToInt(ByVal arr() As Integer) As Integer
			Dim x As Integer = 0
			Dim m As Integer = 1
			For i As Integer = 0 To arr.Length - 1
				If arr(i) = 1 Then
					x += m
				End If
				m *= 2
			Next i
			Return x
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = MyBase.GetHashCode()
			result = 31 * result + (If(varName_Conflict IsNot Nothing, varName_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(variableType <> Nothing, variableType.GetHashCode(), 0))
			result = 31 * result + (If(dataType_Conflict <> Nothing, dataType_Conflict.GetHashCode(), 0))
			Return result
		End Function

		Public Overridable Function clone(ByVal sd As SameDiff) As SDVariable
			Dim v As New SDVariable()
			v.varName_Conflict = varName_Conflict
			v.variableType = variableType
			v.shape_Conflict = If(shape_Conflict Is Nothing, Nothing, CType(shape_Conflict.Clone(), Long()))
			v.dataType_Conflict = dataType_Conflict
			v.sameDiff_Conflict = sd
			Return v
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then
				Return True
			End If
			If Not (TypeOf o Is SDVariable) Then
				Return False
			End If

			Dim s As SDVariable = DirectCast(o, SDVariable)
			If Not varName_Conflict.Equals(s.varName_Conflict) Then
				Return False
			End If
			If variableType <> s.variableType Then
				Return False
			End If
			If dataType_Conflict <> s.dataType_Conflict Then
				Return False
			End If

			If variableType = VariableType.VARIABLE OrElse variableType = VariableType.CONSTANT Then
				Dim a1 As INDArray = Arr
				Dim a2 As INDArray = s.Arr
				Return a1.Equals(a2)
			End If
			Return True
		End Function
	End Class

End Namespace