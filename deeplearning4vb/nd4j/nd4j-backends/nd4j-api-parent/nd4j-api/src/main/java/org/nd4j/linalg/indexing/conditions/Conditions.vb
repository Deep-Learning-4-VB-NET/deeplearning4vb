Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.indexing.conditions

	Public Class Conditions

		''' <summary>
		''' This method will create Condition that checks if value is infinite
		''' @return
		''' </summary>
		Public Shared ReadOnly Property Infinite As Condition
			Get
				Return New IsInfinite()
			End Get
		End Property

		''' <summary>
		''' This method will create Condition that checks if value is NaN
		''' @return
		''' </summary>
		Public Shared ReadOnly Property Nan As Condition
			Get
				Return New IsNaN()
			End Get
		End Property

		''' <summary>
		''' This method will create Condition that checks if value is finite
		''' @return
		''' </summary>
		Public Shared ReadOnly Property Finite As Condition
			Get
				Return New IsFinite()
			End Get
		End Property

		''' <summary>
		''' This method will create Condition that checks if value is NOT finite
		''' @return
		''' </summary>
		Public Shared Function notFinite() As Condition
			Return New NotFinite()
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are not equal wrt eps
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function epsNotEquals() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return epsNotEquals(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are not equal wrt eps
		''' 
		''' @return
		''' </summary>
		Public Shared Function epsNotEquals(ByVal value As Number) As Condition
			Return New EpsilonNotEquals(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are equal wrt eps
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function epsEquals() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return epsEquals(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are equal wrt eps
		''' 
		''' @return
		''' </summary>
		Public Shared Function epsEquals(ByVal value As Number) As Condition
			Return epsEquals(value, Nd4j.EPS_THRESHOLD)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are equal wrt eps
		''' 
		''' @return
		''' </summary>
		Public Shared Function epsEquals(ByVal value As Number, ByVal epsilon As Number) As Condition
			Return New EpsilonEquals(value, epsilon.doubleValue())
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are equal
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function equals() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return equals(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are equal
		''' 
		''' @return
		''' </summary>
		Public Shared Function equals(ByVal value As Number) As Condition
			Return New EqualsCondition(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are not equal
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function notEquals() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return notEquals(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is two values are not equal
		''' 
		''' @return
		''' </summary>
		Public Shared Function notEquals(ByVal value As Number) As Condition
			Return New NotEqualsCondition(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is greater than value Y
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function greaterThan() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return greaterThan(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is greater than value Y
		''' 
		''' @return
		''' </summary>
		Public Shared Function greaterThan(ByVal value As Number) As Condition
			Return New GreaterThan(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is less than value Y
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function lessThan() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return lessThan(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is less than value Y
		''' 
		''' @return
		''' </summary>
		Public Shared Function lessThan(ByVal value As Number) As Condition
			Return New LessThan(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is less than or equal to value Y
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function lessThanOrEqual() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return lessThanOrEqual(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is less than or equal to value Y
		''' 
		''' @return
		''' </summary>
		Public Shared Function lessThanOrEqual(ByVal value As Number) As Condition
			Return New LessThanOrEqual(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is greater than or equal to value Y
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function greaterThanOrEqual() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return greaterThanOrEqual(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is greater than or equal to value Y
		''' 
		''' @return
		''' </summary>
		Public Shared Function greaterThanOrEqual(ByVal value As Number) As Condition
			Return New GreaterThanOrEqual(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is greater than or equal to value Y in absolute values
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function absGreaterThanOrEqual() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return absGreaterThanOrEqual(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is greater than or equal to value Y in absolute values
		''' 
		''' @return
		''' </summary>
		Public Shared Function absGreaterThanOrEqual(ByVal value As Number) As Condition
			Return New AbsValueGreaterOrEqualsThan(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is less than or equal to value Y in absolute values
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function absLessThanOrEqual() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return absLessThanOrEqual(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is less than or equal to value Y in absolute values
		''' 
		''' @return
		''' </summary>
		Public Shared Function absLessThanOrEqual(ByVal value As Number) As Condition
			Return New AbsValueLessOrEqualsThan(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is greater than value Y in absolute values
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function absGreaterThan() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return absGreaterThan(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is greater than value Y in absolute values
		''' 
		''' @return
		''' </summary>
		Public Shared Function absGreaterThan(ByVal value As Number) As Condition
			Return New AbsValueGreaterThan(value)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is less than value Y in absolute values
		''' 
		''' PLEASE NOTE: This condition should be used only with pairwise methods, i.e. INDArray.match(...)
		''' @return
		''' </summary>
		Public Shared Function absLessThan() As Condition
			' in case of pairwise MatchCondition we don't really care about number here
			Return absLessThan(0.0)
		End Function

		''' <summary>
		''' This method will create Condition that checks if value is value X is less than value Y in absolute values
		''' 
		''' @return
		''' </summary>
		Public Shared Function absLessThan(ByVal value As Number) As Condition
			Return New AbsValueLessThan(value)
		End Function

	End Class

End Namespace