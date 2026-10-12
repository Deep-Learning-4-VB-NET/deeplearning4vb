Imports System.IO

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

Namespace org.deeplearning4j.text.documentiterator



	Public Interface DocumentIterator



		''' <summary>
		''' Get the next document </summary>
		''' <returns> the input stream for the next document </returns>
		Function nextDocument() As Stream

		''' <summary>
		''' Whether there are anymore documents in the iterator </summary>
		''' <returns> whether there are anymore documents
		''' in the iterator </returns>
		Function hasNext() As Boolean

		''' <summary>
		''' Reset the iterator to the beginning
		''' </summary>
		Sub reset()



	End Interface

End Namespace