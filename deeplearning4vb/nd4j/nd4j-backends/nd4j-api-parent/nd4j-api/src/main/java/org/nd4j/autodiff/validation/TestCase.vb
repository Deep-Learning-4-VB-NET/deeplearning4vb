Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Accessors = lombok.experimental.Accessors
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports EqualityFn = org.nd4j.autodiff.validation.functions.EqualityFn
Imports RelErrorFn = org.nd4j.autodiff.validation.functions.RelErrorFn
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.function

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

Namespace org.nd4j.autodiff.validation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Accessors(fluent = true) @Getter public class TestCase
	Public Class TestCase
		Public Enum TestSerialization
			BEFORE_EXEC
			AFTER_EXEC
			BOTH
			NONE

		End Enum
		Public Const GC_DEFAULT_PRINT As Boolean = False
		Public Const GC_DEFAULT_EXIT_FIRST_FAILURE As Boolean = False
		Public Const GC_DEFAULT_DEBUG_MODE As Boolean = False
		Public Const GC_DEFAULT_EPS As Double = 1e-5
		Public Const GC_DEFAULT_MAX_REL_ERROR As Double = 1e-5
		Public Const GC_DEFAULT_MIN_ABS_ERROR As Double = 1e-6

		'To test
		Private sameDiff As SameDiff
		Private testName As String

		'Forward pass test configuration
	'    
	'     * Note: These forward pass functions are used to validate the output of forward pass for inputs already set
	'     * on the SameDiff instance.
	'     * Key:     The name of the variable to check the forward pass output for
	'     * Value:   A function to check the correctness of the output
	'     * NOTE: The Function<INDArray,String> should return null on correct results, and an error message otherwise
	'     
		Private fwdTestFns As IDictionary(Of String, [Function](Of INDArray, String))
'JAVA TO VB CONVERTER NOTE: The field placeholderValues was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private placeholderValues_Conflict As IDictionary(Of String, INDArray)

		'Gradient check configuration
		Private gradientCheck As Boolean = True
		Private gradCheckPrint As Boolean = GC_DEFAULT_PRINT
		Private gradCheckDefaultExitFirstFailure As Boolean = GC_DEFAULT_EXIT_FIRST_FAILURE
		Private gradCheckDebugMode As Boolean = GC_DEFAULT_DEBUG_MODE
		Private gradCheckEpsilon As Double = GC_DEFAULT_EPS
		Private gradCheckMaxRelativeError As Double = GC_DEFAULT_MAX_REL_ERROR
		Private gradCheckMinAbsError As Double = GC_DEFAULT_MIN_ABS_ERROR
'JAVA TO VB CONVERTER NOTE: The field gradCheckSkipVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private gradCheckSkipVariables_Conflict As ISet(Of String)
'JAVA TO VB CONVERTER NOTE: The field gradCheckMask was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private gradCheckMask_Conflict As IDictionary(Of String, INDArray)

		'FlatBuffers serialization configuration
		Private testFlatBufferSerialization As TestSerialization = TestSerialization.BOTH


		''' <param name="sameDiff"> SameDiff instance to test. Note: All of the required inputs should already be set </param>
		Public Sub New(ByVal sameDiff As SameDiff)
			Me.sameDiff = sameDiff
		End Sub

		''' <summary>
		''' Validate the output (forward pass) for a single variable using INDArray.equals(INDArray)
		''' </summary>
		''' <param name="name">     Name of the variable to check </param>
		''' <param name="expected"> Expected INDArray </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TestCase expectedOutput(@NonNull String name, @NonNull INDArray expected)
		Public Overridable Function expectedOutput(ByVal name As String, ByVal expected As INDArray) As TestCase
			Return expected(name, New EqualityFn(expected))
		End Function

		''' <summary>
		''' Validate the output (forward pass) for a single variable using element-wise relative error:
		''' relError = abs(x-y)/(abs(x)+abs(y)), with x=y=0 case defined to be 0.0.
		''' Also has a minimum absolute error condition, which must be satisfied for the relative error failure to be considered
		''' legitimate
		''' </summary>
		''' <param name="name">        Name of the variable to check </param>
		''' <param name="expected">    Expected INDArray </param>
		''' <param name="maxRelError"> Maximum allowable relative error </param>
		''' <param name="minAbsError"> Minimum absolute error for a failure to be considered legitimate </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TestCase expectedOutputRelError(@NonNull String name, @NonNull INDArray expected, double maxRelError, double minAbsError)
		Public Overridable Function expectedOutputRelError(ByVal name As String, ByVal expected As INDArray, ByVal maxRelError As Double, ByVal minAbsError As Double) As TestCase
			Return expected(name, New RelErrorFn(expected, maxRelError, minAbsError))
		End Function

		''' <summary>
		''' Validate the output (forward pass) for a single variable using INDArray.equals(INDArray)
		''' </summary>
		''' <param name="var">    Variable to check </param>
		''' <param name="output"> Expected INDArray </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TestCase expected(@NonNull SDVariable var, @NonNull INDArray output)
		Public Overridable Function expected(ByVal var As SDVariable, ByVal output As INDArray) As TestCase
			Return expected(var.name(), output)
		End Function

		''' <summary>
		''' Validate the output (forward pass) for a single variable using INDArray.equals(INDArray)
		''' </summary>
		''' <param name="name">   Name of the variable to check </param>
		''' <param name="output"> Expected INDArray </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TestCase expected(@NonNull String name, @NonNull INDArray output)
		Public Overridable Function expected(ByVal name As String, ByVal output As INDArray) As TestCase
			Return expectedOutput(name, output)
		End Function

		Public Overridable Function expected(ByVal var As SDVariable, ByVal validationFn As [Function](Of INDArray, String)) As TestCase
			Return expected(var.name(), validationFn)
		End Function

		''' <param name="name">         The name of the variable to check </param>
		''' <param name="validationFn"> Function to use to validate the correctness of the specific Op. Should return null
		'''                     if validation passes, or an error message if the op validation fails </param>
		Public Overridable Function expected(ByVal name As String, ByVal validationFn As [Function](Of INDArray, String)) As TestCase
			If fwdTestFns Is Nothing Then
				fwdTestFns = New LinkedHashMap(Of String, [Function](Of INDArray, String))()
			End If
			fwdTestFns(name) = validationFn
			Return Me
		End Function

		Public Overridable Function gradCheckSkipVariables() As ISet(Of String)
			Return gradCheckSkipVariables_Conflict
		End Function

		Public Overridable Function gradCheckMask() As IDictionary(Of String, INDArray)
			Return gradCheckMask_Conflict
		End Function

		''' <summary>
		''' Specify the input variables that should NOT be gradient checked.
		''' For example, if an input is an integer index (not real valued) it should be skipped as such an input cannot
		''' be gradient checked
		''' </summary>
		''' <param name="toSkip"> Name of the input variables to skip gradient check for </param>
		Public Overridable Function gradCheckSkipVariables(ParamArray ByVal toSkip() As String) As TestCase
			If gradCheckSkipVariables_Conflict Is Nothing Then
				gradCheckSkipVariables_Conflict = New LinkedHashSet(Of String)()
			End If
			Collections.addAll(gradCheckSkipVariables_Conflict, toSkip)
			Return Me
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter placeholderValues was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function placeholderValues(ByVal placeholderValues_Conflict As IDictionary(Of String, INDArray)) As TestCase
			Me.placeholderValues_Conflict = placeholderValues_Conflict
			Return Me
		End Function

		Public Overridable Function placeholderValue(ByVal variable As String, ByVal value As INDArray) As TestCase
			If Me.placeholderValues_Conflict Is Nothing Then
				Me.placeholderValues_Conflict = New Dictionary(Of String, INDArray)()
			End If
			Me.placeholderValues_Conflict(variable) = value
			Return Me
		End Function


		Public Overridable Sub assertConfigValid()
			Preconditions.checkNotNull(sameDiff, "SameDiff instance cannot be null%s", testNameErrMsg())
			Preconditions.checkState(gradientCheck OrElse (fwdTestFns IsNot Nothing AndAlso fwdTestFns.Count > 0), "Test case is empty: nothing to test" & " (gradientCheck == false and no expected results available)%s", testNameErrMsg())
		End Sub

		Public Overridable Function testNameErrMsg() As String
			If testName Is Nothing Then
				Return ""
			End If
			Return " - Test name: """ & testName & """"
		End Function

	End Class

End Namespace