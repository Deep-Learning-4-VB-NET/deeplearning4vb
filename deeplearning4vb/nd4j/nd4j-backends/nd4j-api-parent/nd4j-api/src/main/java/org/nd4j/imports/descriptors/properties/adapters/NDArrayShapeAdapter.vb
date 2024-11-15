﻿Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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
'ORIGINAL LINE: @AllArgsConstructor public class NDArrayShapeAdapter implements org.nd4j.imports.descriptors.properties.AttributeAdapter
	Public Class NDArrayShapeAdapter
		Implements AttributeAdapter

		Private index As Integer

		Public Overridable Sub mapAttributeFor(ByVal inputAttributeValue As Object, ByVal fieldFor As System.Reflection.FieldInfo, ByVal [on] As DifferentialFunction) Implements AttributeAdapter.mapAttributeFor
			Dim input As INDArray = DirectCast(inputAttributeValue, INDArray)
			[on].setValueFor(fieldFor,input.size(index))
		End Sub
	End Class

End Namespace