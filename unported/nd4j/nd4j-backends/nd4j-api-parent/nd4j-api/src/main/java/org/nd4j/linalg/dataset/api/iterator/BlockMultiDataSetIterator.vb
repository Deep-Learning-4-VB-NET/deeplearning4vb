Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.nd4j.linalg.dataset.api.iterator

	Public Interface BlockMultiDataSetIterator

		''' <summary>
		''' This method checks, if underlying iterator has at least 1 DataSet
		''' @return
		''' </summary>
		Function hasAnything() As Boolean

		''' <summary>
		''' This method tries to fetch specified number of DataSets and returns them </summary>
		''' <param name="maxDatasets">
		''' @return </param>
		Function [next](ByVal maxDatasets As Integer) As MultiDataSet()
	End Interface

End Namespace