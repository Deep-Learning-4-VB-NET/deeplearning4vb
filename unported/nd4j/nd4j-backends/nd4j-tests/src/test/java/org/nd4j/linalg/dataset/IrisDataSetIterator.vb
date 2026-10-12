Imports System

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

Namespace org.nd4j.linalg.dataset


	<Serializable>
	Public Class IrisDataSetIterator
		Inherits BaseDatasetIterator

		''' 
		Private Const serialVersionUID As Long = -2022454995728680368L

		''' <summary>
		''' IrisDataSetIterator handles
		''' traversing through the Iris Data Set. </summary>
		''' <seealso cref= <a href="https://archive.ics.uci.edu/ml/datasets/Iris">https://archive.ics.uci.edu/ml/datasets/Iris</a>
		''' 
		''' 
		''' Typical usage of an iterator is akin to:
		''' 
		''' DataSetIterator iter = ..;
		''' 
		''' while(iter.hasNext()) {
		'''     DataSet d = iter.next();
		'''     //iterate network...
		''' }
		''' 
		''' 
		''' For custom numbers of examples/batch sizes you can call:
		''' 
		''' iter.next(num)
		''' 
		''' where num is the number of examples to fetch
		'''  </seealso>
		Public Sub New(ByVal batch As Integer, ByVal numExamples As Integer)
			MyBase.New(batch, numExamples, New IrisDataFetcher())
		End Sub



	End Class

End Namespace