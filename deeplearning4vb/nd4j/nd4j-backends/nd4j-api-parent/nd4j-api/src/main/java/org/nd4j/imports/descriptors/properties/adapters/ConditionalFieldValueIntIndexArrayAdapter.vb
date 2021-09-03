﻿Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter

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

Namespace org.nd4j.imports.descriptors.properties.adapters


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class ConditionalFieldValueIntIndexArrayAdapter implements org.nd4j.imports.descriptors.properties.AttributeAdapter
	Public Class ConditionalFieldValueIntIndexArrayAdapter
		Implements AttributeAdapter

		Private targetValue As Object
		Private trueIndex, falseIndex As Integer
		Private fieldName As System.Reflection.FieldInfo

		Public Overridable Sub mapAttributeFor(ByVal inputAttributeValue As Object, ByVal fieldFor As System.Reflection.FieldInfo, ByVal [on] As DifferentialFunction) Implements AttributeAdapter.mapAttributeFor
			Dim inputValue() As Integer = DirectCast(inputAttributeValue, Integer())
			Dim comp As Object = [on].getValue(fieldName)
			If targetValue.Equals(comp) Then
				[on].setValueFor(fieldFor,inputValue(trueIndex))
			Else
				[on].setValueFor(fieldFor,inputValue(falseIndex))
			End If
		End Sub
	End Class

End Namespace