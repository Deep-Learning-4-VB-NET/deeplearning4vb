Imports LongPointer = org.bytedeco.javacpp.LongPointer
Imports PointerPointer = org.bytedeco.javacpp.PointerPointer

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

Namespace org.nd4j.nativeblas

	''' <summary>
	''' Wrapper for DoublePointer -> LongPointer
	''' </summary>
	Public Class PointerPointerWrapper
		Inherits PointerPointer

		Public Sub New(ByVal pointer As LongPointer)
			Me.address = pointer.address()
			Me.capacity = pointer.capacity()
			Me.limit = pointer.limit()
			Me.position = pointer.position()
		End Sub
	End Class

End Namespace