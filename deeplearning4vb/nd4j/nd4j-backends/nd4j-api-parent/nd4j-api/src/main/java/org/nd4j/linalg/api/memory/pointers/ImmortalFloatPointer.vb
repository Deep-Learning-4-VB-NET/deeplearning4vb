Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports Pointer = org.bytedeco.javacpp.Pointer

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

Namespace org.nd4j.linalg.api.memory.pointers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ImmortalFloatPointer extends org.bytedeco.javacpp.FloatPointer
	Public Class ImmortalFloatPointer
		Inherits FloatPointer

		Private pointer As Pointer

		Public Sub New(ByVal pointer As PagedPointer)
			Me.pointer = pointer

			Me.address = pointer.address()
			Me.capacity = pointer.capacity()
			Me.limit = pointer.limit()
			Me.position = 0
		End Sub
	End Class

End Namespace