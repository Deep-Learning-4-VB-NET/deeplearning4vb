Imports System
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff

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

Namespace org.nd4j.autodiff.samediff.transform

	Public MustInherit Class OpPredicate

		''' 
		''' <param name="sameDiff"> SameDiff instance the function belongs to </param>
		''' <param name="function"> </param>
		''' <returns> Returns whether the specific function matches the predicate </returns>
		Public MustOverride Function matches(ByVal sameDiff As SameDiff, ByVal [function] As DifferentialFunction) As Boolean


		''' <summary>
		''' Return true if the operation own (user specified) name equals the specified name
		''' </summary>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static OpPredicate nameEquals(final String name)
		Public Shared Function nameEquals(ByVal name As String) As OpPredicate
			Return New OpPredicateAnonymousInnerClass(name)
		End Function

		Private Class OpPredicateAnonymousInnerClass
			Inherits OpPredicate

			Private name As String

			Public Sub New(ByVal name As String)
				Me.name = name
			End Sub

			Public Overrides Function matches(ByVal sameDiff As SameDiff, ByVal [function] As DifferentialFunction) As Boolean
				Return [function].getOwnName().Equals(name)
			End Function
		End Class

		''' <summary>
		''' Return true if the operation name (i.e., "add", "mul", etc - not the user specified name) equals the specified name
		''' </summary>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static OpPredicate opNameEquals(final String opName)
		Public Shared Function opNameEquals(ByVal opName As String) As OpPredicate
			Return New OpPredicateAnonymousInnerClass2(opName)
		End Function

		Private Class OpPredicateAnonymousInnerClass2
			Inherits OpPredicate

			Private opName As String

			Public Sub New(ByVal opName As String)
				Me.opName = opName
			End Sub

			Public Overrides Function matches(ByVal sameDiff As SameDiff, ByVal [function] As DifferentialFunction) As Boolean
				Return [function].opName().Equals(opName)
			End Function
		End Class

		''' <summary>
		''' Return true if the operation own (user specified) name matches the specified regular expression
		''' </summary>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static OpPredicate nameMatches(final String regex)
		Public Shared Function nameMatches(ByVal regex As String) As OpPredicate
			Return New OpPredicateAnonymousInnerClass3(regex)
		End Function

		Private Class OpPredicateAnonymousInnerClass3
			Inherits OpPredicate

			Private regex As String

			Public Sub New(ByVal regex As String)
				Me.regex = regex
			End Sub

			Public Overrides Function matches(ByVal sameDiff As SameDiff, ByVal [function] As DifferentialFunction) As Boolean
				Return [function].getOwnName().matches(regex)
			End Function
		End Class

		''' <summary>
		''' Return true if the operation name (i.e., "add", "mul", etc - not the user specified name) matches the specified regular expression
		''' </summary>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static OpPredicate opNameMatches(final String regex)
		Public Shared Function opNameMatches(ByVal regex As String) As OpPredicate
			Return New OpPredicateAnonymousInnerClass4(regex)
		End Function

		Private Class OpPredicateAnonymousInnerClass4
			Inherits OpPredicate

			Private regex As String

			Public Sub New(ByVal regex As String)
				Me.regex = regex
			End Sub

			Public Overrides Function matches(ByVal sameDiff As SameDiff, ByVal [function] As DifferentialFunction) As Boolean
				Return [function].getOwnName().matches(regex)
			End Function
		End Class

		''' <summary>
		''' Return true if the operation class is equal to the specified class
		''' </summary>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static OpPredicate classEquals(final @Class c)
		Public Shared Function classEquals(ByVal c As Type) As OpPredicate
			Return New OpPredicateAnonymousInnerClass5(c)
		End Function

		Private Class OpPredicateAnonymousInnerClass5
			Inherits OpPredicate

			Private c As Type

			Public Sub New(ByVal c As Type)
				Me.c = c
			End Sub

			Public Overrides Function matches(ByVal sameDiff As SameDiff, ByVal [function] As DifferentialFunction) As Boolean
				Return [function].GetType() = c
			End Function
		End Class


	End Class

End Namespace