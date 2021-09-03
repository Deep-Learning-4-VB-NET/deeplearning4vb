Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.jita.allocator.impl

	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class AllocationShape
	Public Class AllocationShape
		Private length As Long = 0
'JAVA TO VB CONVERTER NOTE: The field elementSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private elementSize_Conflict As SByte = 0
		Private dataType As DataType = DataType.FLOAT

	'    
	'    public AllocationShape(long length, int elementSize) {
	'        this.length = length;
	'        this.elementSize = elementSize;
	'    }
	'    
		Public Sub New(ByVal length As Long, ByVal elementSize As Integer, ByVal dataType As DataType)
			Me.length = length
			Me.elementSize_Conflict = CSByte(elementSize)
			Me.dataType = dataType
		End Sub

		Public Overridable Property ElementSize As Integer
			Get
				Return elementSize_Conflict
			End Get
			Set(ByVal elementSize As Integer)
				Me.elementSize_Conflict = CSByte(elementSize)
			End Set
		End Property



		Public Overridable ReadOnly Property NumberOfBytes As Long
			Get
				Return Me.length * Me.elementSize_Conflict
			End Get
		End Property
	End Class

End Namespace