﻿Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

	Public Interface ParallelMultiDataSetIterator
		Inherits MultiDataSetIterator

		''' <summary>
		''' This method sets consumer affinity to specific producer
		''' 
		''' PLEASE NOTE: this method is optional, and it'll change only nextFor()/hasNextFor() mechanics
		''' </summary>
		Sub attachThread(ByVal producer As Integer)

		''' <summary>
		''' Returns true, if attached producer has something in queue, false otherwise
		''' 
		''' @return
		''' </summary>
		Function hasNextFor() As Boolean

		''' <summary>
		''' Returns true, if attached producer has something in queue, false otherwise
		''' </summary>
		''' <param name="consumer">
		''' @return </param>
		Function hasNextFor(ByVal consumer As Integer) As Boolean

		''' <summary>
		''' Returns next DataSet for given consumer
		''' </summary>
		''' <param name="consumer">
		''' @return </param>
		Function nextFor(ByVal consumer As Integer) As MultiDataSet

		''' <summary>
		''' Returns next DataSet for attached consumer
		''' 
		''' @return
		''' </summary>
		Function nextFor() As MultiDataSet
	End Interface

End Namespace