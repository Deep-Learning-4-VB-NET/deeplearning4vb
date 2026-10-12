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

Namespace org.deeplearning4j.parallelism.inference

	Public Enum InferenceMode
		''' <summary>
		''' input will be passed into the model as is
		''' </summary>
		SEQUENTIAL

		''' <summary>
		''' input will be included into the batch if computation device is busy, and executed immediately otherwise
		''' </summary>
		BATCHED

		''' <summary>
		''' Inference will applied in the calling thread instead of workers. Worker models will be using shared parameters on per-device basis.
		''' </summary>
		INPLACE
	End Enum

End Namespace