Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Accessors = lombok.experimental.Accessors
Imports EqualityFn = org.nd4j.autodiff.validation.functions.EqualityFn
Imports RelErrorFn = org.nd4j.autodiff.validation.functions.RelErrorFn
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
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
'ORIGINAL LINE: @Data @Accessors(fluent = true) public class OpTestCase
	Public Class OpTestCase

		Private ReadOnly op As DynamicCustomOp
		Private testFns As IDictionary(Of Integer, [Function](Of INDArray, String)) = New LinkedHashMap(Of Integer, [Function](Of INDArray, String))()
		Private expShapes As IDictionary(Of Integer, LongShapeDescriptor) = New Dictionary(Of Integer, LongShapeDescriptor)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OpTestCase(@NonNull DynamicCustomOp op)
		Public Sub New(ByVal op As DynamicCustomOp)
			Me.op = op
		End Sub

		''' <summary>
		''' Validate the op output using INDArray.equals(INDArray)
		''' </summary>
		''' <param name="outputNum"> Number of the output </param>
		''' <param name="expected">  Expected INDArray </param>
		Public Overridable Function expectedOutput(ByVal outputNum As Integer, ByVal expected As INDArray) As OpTestCase
			testFns(outputNum) = New EqualityFn(expected)
			expShapes(outputNum) = expected.shapeDescriptor()
			Return Me
		End Function

		''' <summary>
		''' Validate the output for a single variable using element-wise relative error:
		''' relError = abs(x-y)/(abs(x)+abs(y)), with x=y=0 case defined to be 0.0.
		''' Also has a minimum absolute error condition, which must be satisfied for the relative error failure to be considered
		''' legitimate
		''' </summary>
		''' <param name="outputNum">   output number </param>
		''' <param name="expected">    Expected INDArray </param>
		''' <param name="maxRelError"> Maximum allowable relative error </param>
		''' <param name="minAbsError"> Minimum absolute error for a failure to be considered legitimate </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OpTestCase expectedOutputRelError(int outputNum, @NonNull INDArray expected, double maxRelError, double minAbsError)
		Public Overridable Function expectedOutputRelError(ByVal outputNum As Integer, ByVal expected As INDArray, ByVal maxRelError As Double, ByVal minAbsError As Double) As OpTestCase
			testFns(outputNum) = New RelErrorFn(expected, maxRelError, minAbsError)
			expShapes(outputNum) = expected.shapeDescriptor()
			Return Me
		End Function

		''' <param name="outputNum">    Output number to check </param>
		''' <param name="expShape">     Expected shape for the output </param>
		''' <param name="validationFn"> Function to use to validate the correctness of the specific Op. Should return null
		'''                     if validation passes, or an error message if the op validation fails </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OpTestCase expectedOutput(int outputNum, @NonNull LongShapeDescriptor expShape, @NonNull @Function<org.nd4j.linalg.api.ndarray.INDArray, String> validationFn)
		Public Overridable Function expectedOutput(ByVal outputNum As Integer, ByVal expShape As LongShapeDescriptor, ByVal validationFn As [Function](Of INDArray, String)) As OpTestCase
			testFns(outputNum) = validationFn
			expShapes(outputNum) = expShape
			Return Me
		End Function
	End Class

End Namespace