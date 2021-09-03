Imports System
Imports JsonDeserializerAtomicDouble = org.nd4j.common.primitives.serde.JsonDeserializerAtomicDouble
Imports JsonSerializerAtomicDouble = org.nd4j.common.primitives.serde.JsonSerializerAtomicDouble
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize

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

Namespace org.nd4j.common.primitives

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = JsonSerializerAtomicDouble.class) @JsonDeserialize(using = JsonDeserializerAtomicDouble.class) public class AtomicDouble extends org.nd4j.shade.guava.util.concurrent.AtomicDouble
	Public Class AtomicDouble
		Inherits org.nd4j.shade.guava.util.concurrent.AtomicDouble

		Public Sub New()
			Me.New(0.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AtomicDouble(@JsonProperty("value") double value)
		Public Sub New(ByVal value As Double)
			MyBase.New(value)
		End Sub

		Public Sub New(ByVal value As Single)
			Me.New(CDbl(value))
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			'NOTE: org.nd4j.shade.guava.util.concurrent.AtomicDouble extends Number, hence this class extends number
			If TypeOf o Is Number Then
				Return get() = DirectCast(o, Number).doubleValue()
			End If
			Return False
		End Function

		Public Overrides Function GetHashCode() As Integer
			'return Double.hashCode(get());    //Java 8+
			Return Convert.ToDouble(get()).GetHashCode()
		End Function
	End Class

End Namespace